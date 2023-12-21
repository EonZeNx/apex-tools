using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantStr : APropertyV03, IToApex, IGetValue<string>
{
    public string Value { get; set; } = "";
    public override string XmlName => "Str";

    public VariantStr()
    {
        Header.VariantType = EVariantType.String;
    }
    public VariantStr(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Value = br.ReadStringZ();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);
        xw.WriteValue(Value);
        
        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
        Value = xr.ReadString();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.WriteStringZ(Value);
    }

    public string GetValue()
    {
        return Value;
    }
}