using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantStr : SPropertyV03
{
    public string Value { get; set; } = "";
    public override string XmlName => "Str";

    public VariantStr() { }
    public VariantStr(SPropertyHeaderV03 header) : base(header)
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
}