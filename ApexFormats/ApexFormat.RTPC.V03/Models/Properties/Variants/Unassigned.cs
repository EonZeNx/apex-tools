using System.Xml;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models.Properties.Variants;

public class Unassigned : PropertyBaseV03
{
    public override string XmlName => "Unassigned";
    public override EVariantType VariantType => EVariantType.Unassigned;
    public override bool Primitive => true;
    
    public uint Value { get; set; }


    public Unassigned() { }
    public Unassigned(PropertyHeaderV03 header)
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
        Value = uint.Parse(xr.ReadElementContentAsString());
    }

    public override void ToXml(XmlWriter xw)
    {
        if (Settings.RtpcSkipUnassignedProperties.Value) return;
        
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }
    
    #endregion
}