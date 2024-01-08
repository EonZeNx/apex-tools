using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineFloat : IApexXElementIO
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.Float32;
    protected float Value { get; set; }

    public InlineFloat(InlinePropertyHeader header)
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

        Value = br.ReadSingle();
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
}