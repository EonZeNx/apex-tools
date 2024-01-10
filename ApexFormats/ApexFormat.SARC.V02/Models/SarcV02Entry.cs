using System.Xml.Linq;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Abstractions.Serializable;
using ApexTools.Core.Extensions;
using ApexTools.Core.Interfaces;

namespace ApexFormat.SARC.V02.Models;

public static class SarcV02EntryConstants
{
    public const uint Alignment = 0x04;
}

/// <summary>
/// Structure:
/// <br/>Header - <see cref="SarcV02EntryHeader"/>
/// <br/>Data (Deferred) - <see cref="byte"/>[]
/// </summary>
public class SarcV02Entry : IApexSerializable, ICustomPathSerializable, IToXml, IToApexSerializableDeferred
{
    public SarcV02EntryHeader Header = new();
    public byte[] Data { get; set; } = Array.Empty<byte>();
    
    public static string XmlName => "entry";
    
    public uint SizeOf() => 4 + Header.PathLength + 4 + 4;

    public SarcV02Entry() {}

    public SarcV02Entry(SarcV02EntryHeader header)
    {
        Header = header;
    }


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        if (Header.IsReference()) return;
        
        br.BaseStream.Seek(Header.DataOffset, SeekOrigin.Begin);
        Data = br.ReadBytes((int) Header.Size);
    }

    public void ToApexDeferred(BinaryWriter bw)
    {
        if (Header.IsReference()) return;
        
        Header.DataOffset = (uint) bw.Position();
        bw.Write(Data);
        
        bw.Align(SarcV02EntryConstants.Alignment, 0x00);
    }

    public void ToApex(BinaryWriter bw)
    {
        Header.ToApex(bw);
    }

    #endregion


    #region CustomPathSerializable

    public void FromCustomFile(string basePath)
    {
        if (Header.IsReference()) return;

        var filePath = Path.Join(basePath, Header.SafeFilePath());
        using (var br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
        {
            Data = br.ReadBytes((int) br.BaseStream.Length);
        }
        
        Header.Size = (uint) Data.Length;
    }

    public void ToCustomFile(string basePath)
    {
        if (Header.IsReference()) return;
        
        var directoryPath = Path.Join(basePath, Path.GetDirectoryName(Header.FilePath));
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Join(directoryPath, Path.GetFileName(Header.SafeFilePath()));
        using var bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
        bw.Write(Data);
    }

    #endregion


    #region XmlSerializable

    public XElement ToXml()
    {
        return Header.ToXml();
    }

    #endregion
}