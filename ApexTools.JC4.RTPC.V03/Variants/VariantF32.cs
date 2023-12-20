using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantF32 : APropertyV03
{
    public float Value { get; set; }
    public override string XmlName => "F32";

    public VariantF32()
    {
        Header.VariantType = EVariantType.Float32;
    }
    public VariantF32(PropertyHeaderV03 header) : base(header)
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

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = float.Parse(xr.ReadString());
    }

    public override void ToApex(BinaryWriter bw)
    {
        // Left blank, ToApexHeader will write in the values, this will do nothing
    }
}