using System.Xml;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class Unassigned : PropertyBaseV01
{
    public override string XmlName => "Unassigned";
    public override EVariantType VariantType => EVariantType.Unassigned;
    public override bool Primitive => true;
    
    public uint Value { get; set; }


    public Unassigned() { }
    public Unassigned(PropertyHeaderV01 header)
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
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = uint.Parse(xr.ReadString());
    }

    public override void ToXml(XmlWriter xw)
    {
        if (Settings.SkipUnassignedRtpcProperties.Value) return;
        
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
    
    #endregion
}