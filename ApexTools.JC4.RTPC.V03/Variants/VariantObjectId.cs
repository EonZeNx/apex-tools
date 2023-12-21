using System.Globalization;
using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantObjectId : APropertyV03, IGetValue<(ulong, byte)>
{
    public (ulong, byte) Value { get; set; }
    public override string XmlName => "OID";

    public VariantObjectId()
    {
        Header.VariantType = EVariantType.ObjectId;
    }
    public VariantObjectId(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);
        
        var oid = br.ReadUInt64();
        var userData = (byte) (ByteUtils.ReverseBytes(oid) & byte.MaxValue);
            
        Value = (oid, userData);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);

        var stringOid = ByteUtils.ToHex(Value.Item1);
        var stringUserData = ByteUtils.ToHex(Value.Item2);
            
        var full = $"{stringOid}={stringUserData}";
        xw.WriteValue(full);
        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
            
        var strValue = xr.ReadString();
        var strArray = strValue.Split("=");

        var oid = ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier);
        var userData = byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier);
            
        Value = (oid, userData);
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value.Item1);
    }

    public (ulong, byte) GetValue()
    {
        return Value;
    }
}