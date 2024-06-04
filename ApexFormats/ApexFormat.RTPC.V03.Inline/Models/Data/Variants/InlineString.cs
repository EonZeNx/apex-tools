using System.Text;
using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Extensions;
using ApexTools.Core.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineString : IApexXElementIO
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.String;
    protected string Value { get; set; } = string.Empty;

    public InlineString() {}
    public InlineString(InlinePropertyHeader header)
    {
        NameHash = header.NameHash;
    }

    public void LookupHash()
    {
        Name = LookupHashes.Get(NameHash);
    }

    public void FromApex(BinaryReader br)
    {
        if (NameHash == 0)
        {
            NameHash = br.ReadUInt32();
            br.ReadByte();
        }
        
        var length = br.ReadUInt16();
        var byteString = new byte[length];
        
        for (var i = 0; i < length; i++)
        {
            byteString[i] = br.ReadByte();
        }
        
        Value = Encoding.UTF8.GetString(byteString);
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) VariantType);
        
        bw.Write((ushort) Value.Length);
        bw.Write(Encoding.UTF8.GetBytes(Value));
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
        Value = xe.Value;
    }
}