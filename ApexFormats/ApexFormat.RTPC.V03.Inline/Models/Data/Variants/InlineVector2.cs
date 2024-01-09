using System.Xml.Linq;
using System.Xml.Schema;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineVector2 : InlineCountable
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.Vector2;
    protected float[] Values { get; set; } = Array.Empty<float>();
    
    public override uint Count { get; set; } = 2;

    public InlineVector2() {}
    public InlineVector2(InlinePropertyHeader header)
    {
        NameHash = header.NameHash;
    }

    public override void LookupHash()
    {
        Name = HashUtils.Lookup(NameHash);
    }

    public override void FromApex(BinaryReader br)
    {
        if (NameHash == 0)
        {
            NameHash = br.ReadUInt32();
            br.ReadByte();
        }

        Values = new float[Count];
        for (var i = 0; i < Values.Length; i++)
        {
            Values[i] = br.ReadSingle();
        }
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) VariantType);
        
        foreach (var value in Values)
        {
            bw.Write(value);
        }
    }

    public override XElement ToXElement()
    {
        var xe = new XElement(VariantType.GetXmlName());
        xe.WriteNameOrHash(NameHash, Name);
        
        xe.SetValue(string.Join(",", Values));

        return xe;
    }

    public override void FromXElement(XElement xe)
    {
        NameHash = xe.GetNameHash();
        
        var strValues = xe.Value.Split(",");
        Values = Array.ConvertAll(strValues, float.Parse);

        if (Values.Length != Count)
        {
            throw new XmlSchemaException($"{xe.Value} has too many values, not a valid {VariantType.GetXmlName()}");
        }
    }
}