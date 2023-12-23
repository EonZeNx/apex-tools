using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantU32 : APropertyV03
{
    public uint Value { get; set; }
    public override string XmlName => "U32";

    public VariantU32()
    {
        Header.VariantType = EVariantType.UInteger32;
    }
    public VariantU32(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        Value = BitConverter.ToUInt32(Header.RawData);
    }
    
    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
        Value = uint.Parse(xr.ReadElementContentAsString());
        Header.RawData = BitConverter.GetBytes(Value);
    }

    public override void ToApex(BinaryWriter bw)
    {
        // Left blank, ToApexHeader will write in the values, this will do nothing
    }
}