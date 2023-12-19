using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantU32 : SPropertyV03
{
    public uint Value { get; set; }
    public override string XmlName => "U32";

    public VariantU32() { }
    public VariantU32(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        Value = BitConverter.ToUInt32(Header.RawData);
    }
    
    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
}