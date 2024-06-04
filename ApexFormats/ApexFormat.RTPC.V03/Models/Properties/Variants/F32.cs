using System.Xml;
using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models.Properties.Variants;

public class F32 : PropertyBaseV03
{
    public override string XmlName => "Float";
    public override EVariantType VariantType => EVariantType.Float32;
    public override bool Primitive => true;
    
    public float Value { get; set; }


    public F32() { }
    public F32(PropertyHeaderV03 header)
    {
        NameHash = header.NameHash;
        RawData = header.RawData;
    }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        Value = BitConverter.ToSingle(RawData);
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
        Value = float.Parse(xr.ReadElementContentAsString());
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