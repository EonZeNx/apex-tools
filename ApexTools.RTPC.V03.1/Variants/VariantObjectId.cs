using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantObjectId : SPropertyV03
{
    public (ulong, byte) Value { get; set; }
    public override string XmlName => "OID";

    public VariantObjectId() { }
    public VariantObjectId(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);
        
        var oid = ByteUtils.ReverseBytes(br.ReadUInt64());
        var userData = (byte) (oid & byte.MaxValue);
            
        Value = (oid, userData);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);

        var reversedOid = ByteUtils.ReverseBytes(Value.Item1);
        var stringOid = ByteUtils.ToHex(reversedOid);

        var stringUserData = ByteUtils.ToHex(Value.Item2);
            
        var full = $"{stringOid}={stringUserData}";
        xw.WriteValue(full);
        xw.WriteEndElement();
    }
}