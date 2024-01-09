using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineUInt32 : IApexXElementIO
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.UInteger32;
    protected uint Value { get; set; }

    public InlineUInt32() {}
    public InlineUInt32(InlinePropertyHeader header)
    {
        NameHash = header.NameHash;
    }

    public void LookupHash()
    {
        Name = HashUtils.Lookup(NameHash);
    }

    public void FromApex(BinaryReader br)
    {
        if (NameHash == 0)
        {
            NameHash = br.ReadUInt32();
            br.ReadByte();
        }

        Value = br.ReadUInt32();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) VariantType);
        
        bw.Write(Value);
    }

    public XElement ToXElement()
    {
        var xe = new XElement(VariantType.GetXmlName());
        xe.WriteNameOrHash(NameHash, Name);
        
        xe.SetValue(Value);

        return xe;
    }

    public void FromXElement(XElement xe)
    {
        NameHash = xe.GetNameHash();

        if (!uint.TryParse(xe.Value, out var result))
        {
            throw new XmlSchemaException($"{xe.Value} is not a valid {VariantType.GetXmlName()}");
        }

        Value = result;
    }
}