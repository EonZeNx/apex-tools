using System.Text;
using System.Xml.Linq;
using ApexTools.Core.Extensions;
using ApexTools.Core.Interfaces;
using ApexTools.Core.Utils;

namespace ApexFormat.SARC.V02.Models;

public static class SarcV02EntryHeaderConstants
{
    public const uint FilePathAlignment = 0x04;
}

/// <summary>
/// Structure:
/// <br/>FilePath Length - <see cref="uint"/>
/// <br/>FilePath - <see cref="string"/>
/// <br/>Data offset - <see cref="uint"/>
/// <br/>Size - <see cref="uint"/>
/// </summary>
public class SarcV02EntryHeader : IToApex, IXmlFile
{
    public uint PathLength; // Includes 4-byte Z-pad
    public string FilePath { get; set; } = "";
    public uint DataOffset;
    public uint Size;

    public static string XmlName => "entry";
    public bool IsReference() => DataOffset == 0;
    
    public string SafeFilePath()
    {
        var result = FilePath
            .Replace("\0", "");
        
        return result;
    }
    public void ToApex(BinaryWriter bw)
    {
        var nulls = new string('\0', (int) (PathLength - FilePath.Length));
            
        bw.Write(PathLength);
        bw.Write(Encoding.UTF8.GetBytes(FilePath));
        bw.Write(Encoding.UTF8.GetBytes(nulls));
        bw.Write(DataOffset);
        bw.Write(Size);
    }

    public void FromXml(XElement xe)
    {
        throw new NotImplementedException();
    }
    public XElement ToXml()
    {
        var xe = new XElement(XmlName);

        if (IsReference())
        {
            xe.SetAttributeValue("reference", "1");
            xe.SetAttributeValue("value", $"{SafeFilePath()}");
            xe.SetAttributeValue($"{nameof(Size).ToLower()}", $"{Size}");

            return xe;
        }

        xe.SetAttributeValue("value", $"{SafeFilePath()}");
        
        return xe;
    }
}

public static class SarcV02EntryHeaderExtensions
{
    public static SarcV02EntryHeader ReadSarcV02EntryHeader(this BinaryReader br)
    {
        var result = new SarcV02EntryHeader();
        result.PathLength = br.ReadUInt32();
        result.FilePath = br.ReadStringOfLength(result.PathLength);
        result.DataOffset = br.ReadUInt32();
        result.Size = br.ReadUInt32();

        return result;
    }
    
    public static SarcV02EntryHeader ReadSarcV02EntryHeader(this XElement xe)
    {
        var result = new SarcV02EntryHeader();
        result.FilePath = xe.GetAttribute("value");
        result.PathLength = (uint) ByteUtils.Align(result.FilePath.Length, SarcV02EntryHeaderConstants.FilePathAlignment);
        result.DataOffset = 0xFFFFFFFF; // Only used to pass the IsReference check

        if (!string.IsNullOrEmpty(xe.TryGetAttribute("reference")))
        {
            result.DataOffset = 0x00;
            result.Size = uint.Parse(xe.GetAttribute("size"));
        }

        return result;
    }
}