using System.Text;
using System.Xml;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Abstractions.Serializable;
using ApexTools.Core.Utils;

namespace ApexFormat.SARC.V02.Models;

/// <summary>
/// The structure for <see cref="EntryV02"/>.
/// <br/> FilePath Length - <see cref="uint"/>
/// <br/> FilePath - <see cref="string"/>
/// <br/> Data offset - <see cref="uint"/>
/// <br/> Size - <see cref="uint"/>
/// <br/> Data (Deferred) - <see cref="byte"/>[]
/// </summary>
public class EntryV02: IApexSerializable, ICustomPathSerializable, IToApexSerializableDeferred
{
    public uint PathLength { get; set; }
    public string FilePath { get; set; } = "";
    public uint DataOffset { get; set; }
    public uint Size { get; set; }
    public bool IsReference { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    
    public uint HeaderSize
    {
        get
        {
            var pathLengthWithNulls = ByteUtils.Align(PathLength, 4);
            return 4 + pathLengthWithNulls + 4 + 4;
        }
    }


    public EntryV02() {}

    public EntryV02(XmlReader xr, string basePath)
    {
        DataOffset = 0;
        IsReference = bool.Parse(XmlUtils.GetAttribute(xr, "IsReference"));
        Size = uint.Parse(XmlUtils.GetAttribute(xr, "Size"));
            
        FilePath = xr.ReadElementContentAsString().Replace("\\", "/");
        PathLength = (uint) FilePath.Length;
        
        if (IsReference) return;
        
        var fullPath = Path.Combine(basePath, FilePath);
        using (var br = new BinaryReader(new FileStream(fullPath, FileMode.Open)))
        {
            Data = br.ReadBytes((int) br.BaseStream.Length);
        }
            
        Size = (uint) Data.Length;
    }
    

    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        PathLength = br.ReadUInt32();
            
        FilePath = Encoding.UTF8.GetString(br.ReadBytes((int) PathLength))
            .Replace("/", @"\")
            .Replace("\0", "");
            
        DataOffset = br.ReadUInt32();
        IsReference = DataOffset == 0;
        Size = br.ReadUInt32();

        if (IsReference) return;
            
        var originalPosition = br.Position();
        br.BaseStream.Seek(DataOffset, SeekOrigin.Begin);

        Data = br.ReadBytes((int) Size);
        br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
    }

    public void ToApexDeferred(BinaryWriter bw)
    {
        if (IsReference) return;
            
        DataOffset = (uint) bw.Position();
        bw.Write(Data);
            
        bw.Align(4, 0x00);
    }

    public void ToApex(BinaryWriter bw)
    {
        // Align to 4 bytes
        var pathLengthWithNulls = ByteUtils.Align(PathLength, 4);
        var nulls = new string('\0', (int) (pathLengthWithNulls - PathLength));
            
        bw.Write(pathLengthWithNulls);
        bw.Write(Encoding.UTF8.GetBytes(FilePath));
        bw.Write(Encoding.UTF8.GetBytes(nulls));
        bw.Write(DataOffset);
        bw.Write(Size);
    }

    #endregion


    #region CustomPathSerializable

    public void FromCustomFile(string basePath)
    {
        if (IsReference) return;
            
        using (var br = new BinaryReader(new FileStream(basePath, FileMode.Open)))
        {
            Data = br.ReadBytes((int) br.BaseStream.Length);
        }
        
        Size = (uint) Data.Length;
    }

    public void ToCustomFile(string basePath)
    {
        if (IsReference) return;
        
        // Create the directory from basePath + RelativeFilePath - FileName
        var directoryPath = string.Join(@"\", FilePath.Split(@"\")[..^1]);
        Directory.CreateDirectory(@$"{basePath}\{directoryPath}");

        using var bw = new BinaryWriter(new FileStream(@$"{basePath}\{FilePath}", FileMode.Create));
        bw.Write(Data);
    }

    #endregion


    #region XmlSerializable

    public void XmlWrite(XmlWriter xw)
    {
        // Only write references, can just gather contents of folder for internal files
        // if (!entry.IsReference) return;
            
        xw.WriteStartElement("Entry");
        xw.WriteAttributeString("IsReference", $"{IsReference}");
        xw.WriteAttributeString("Size", $"{Size}");
        xw.WriteValue(FilePath);
        xw.WriteEndElement();
    }

    #endregion
}