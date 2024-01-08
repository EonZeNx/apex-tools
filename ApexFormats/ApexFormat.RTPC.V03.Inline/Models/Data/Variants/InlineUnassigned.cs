using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineUnassigned : IApexXElementIO
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.Unassigned;
    public static string XmlName => "Un";

    public InlineUnassigned(InlinePropertyHeader header)
    {
        NameHash = header.NameHash;
    }

    public void LookupHash()
    {
        Name = HashUtils.Lookup(NameHash);
    }

    public void FromApex(BinaryReader br)
    {
        if (NameHash != 0) return;
        
        NameHash = br.ReadUInt32();
        br.ReadByte();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) VariantType);
    }

    public XElement ToXElement()
    {
        var xe = new XElement(XmlName);
        xe.WriteNameOrHash(NameHash, Name);

        return xe;
    }
}