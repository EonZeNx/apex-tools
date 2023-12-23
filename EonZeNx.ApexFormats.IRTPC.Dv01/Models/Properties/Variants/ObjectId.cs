using System.Globalization;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class ObjectId : PropertyBase
{
    public override string XmlName => "OID";
    public override EVariantType VariantType => EVariantType.ObjectId;
    
    public (ulong, byte) Value { get; set; }


    public ObjectId() { }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        // Thanks UnknownMiscreant
        var oid = ByteUtils.ReverseBytes(br.ReadUInt64());
        var userData = (byte) (oid & byte.MaxValue);
            
        Value = (oid, userData);
    }

    public override void ToApex(BinaryWriter bw)
    {
        base.ToApex(bw);
        bw.Write(Value.Item1);
    }

    #endregion

    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var strValue = xr.ReadElementContentAsString();
        var strArray = strValue.Split("=");

        var reversedOid = ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier);
        var oid = ByteUtils.ReverseBytes(reversedOid);

        var userData = byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier);
            
        Value = (oid, userData);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

        var reversedOid = ByteUtils.ReverseBytes(Value.Item1);
        var stringOid = ByteUtils.ToHex(reversedOid);

        var stringUserData = ByteUtils.ToHex(Value.Item2);
            
        var full = $"{stringOid}={stringUserData}";
        xw.WriteValue(full);
        xw.WriteEndElement();
    }
    
    #endregion
}