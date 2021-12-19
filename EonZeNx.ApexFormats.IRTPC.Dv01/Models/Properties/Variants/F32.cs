using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class F32 : PropertyBase
{
    public override string XmlName => "Float";
    public override EVariantType VariantType => EVariantType.Float32;
    
    public float Value { get; set; }


    public F32() { }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadSingle();
    }

    public override void ToApex(BinaryWriter bw)
    {
        base.ToApex(bw);
        bw.Write(Value);
    }

    #endregion

    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = float.Parse(xr.ReadString());
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
    
    #endregion
}