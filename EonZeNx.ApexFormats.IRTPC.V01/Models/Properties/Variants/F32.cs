using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class F32 : BasePropertyV01
{
    public override string XmlName => "F32";
    protected override EVariantType VariantType => EVariantType.Float32;

    protected float Value { get; set; }


    public F32() { }
    public F32(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01) { }
    
    
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
        xw.WriteAttributeString("Offset", ByteUtils.ToHex((uint) Offset));
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    #endregion
}