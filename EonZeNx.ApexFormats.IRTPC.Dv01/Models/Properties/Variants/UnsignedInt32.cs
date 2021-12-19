using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class UnsignedInt32 : PropertyBase
{
    public override string XmlName => "UInt32";
    public override EVariantType VariantType => EVariantType.UInteger32;
    
    public uint Value { get; set; }


    public UnsignedInt32() { }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadUInt32();
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
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = uint.Parse(xr.ReadString());
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