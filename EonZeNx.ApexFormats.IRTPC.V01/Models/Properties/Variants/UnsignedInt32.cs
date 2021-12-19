using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class UnsignedInt32 : BasePropertyV01
{
    public override string XmlName => "UInt32";
    protected override EVariantType VariantType => EVariantType.UInteger32;
    protected uint Value { get; set; }


    public UnsignedInt32() { }
    public UnsignedInt32(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01) { }
    
    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadUInt32();
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
        var valueHash = XmlUtils.GetAttribute(xr, "Hash");
        var rawValue = xr.ReadElementContentAsString();
            
        if (string.IsNullOrEmpty(valueHash))
        {
            Value = uint.Parse(rawValue);
        }
        // else
        // {
        //     // Do this instead of error catching and throwing, it's much faster
        //     LookupValue = rawValue;
        //     Value = (uint) HashJenkinsL3.Hash(LookupValue);
        // }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
        
        // TODO: Check if uint is a hash and write if so
        // if (!string.IsNullOrEmpty(LookupValue))
        // {
        //     xw.WriteAttributeString("Hash", $"{Value}");
        //     xw.WriteValue(LookupValue);
        // }
        // else
        // {
        //     xw.WriteValue(Value);
        // }
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    #endregion
}