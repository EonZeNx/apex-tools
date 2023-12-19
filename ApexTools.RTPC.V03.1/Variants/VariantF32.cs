using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantF32 : SPropertyV03
{
    public float Value { get; set; }
    public override string XmlName => "F32";

    public VariantF32() { }
    public VariantF32(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        Value = BitConverter.ToSingle(Header.RawData);
    }
    
    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
}