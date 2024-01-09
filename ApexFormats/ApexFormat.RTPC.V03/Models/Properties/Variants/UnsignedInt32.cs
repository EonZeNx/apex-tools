using System.Xml;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models.Properties.Variants;

public class UnsignedInt32 : PropertyBaseV03
{
    public override string XmlName => "UInt32";
    public override EVariantType VariantType => EVariantType.UInteger32;
    public override bool Primitive => true;
    
    public uint Value { get; set; }


    public UnsignedInt32() { }
    public UnsignedInt32(PropertyHeaderV03 header)
    {
        NameHash = header.NameHash;
        RawData = header.RawData;
    }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        Value = BitConverter.ToUInt32(RawData);
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(Value);
        bw.Write((byte) VariantType);
    }

    #endregion

    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = xr.ReadNameIfValid();
        Value = uint.Parse(xr.ReadElementContentAsString());
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        xw.WriteNameOrNameHash(NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
    
    #endregion
}