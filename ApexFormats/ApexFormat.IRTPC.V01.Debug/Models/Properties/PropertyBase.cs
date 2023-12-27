using System.Xml;
using ApexTools.Core.Abstractions.CombinedSerializable;

namespace ApexFormat.IRTPC.V01.Debug.Models.Properties;

public class PropertyBase : XmlSerializable, IApexSerializable
{
    public override string XmlName => "PropertyBase";
    public virtual EVariantType VariantType { get; }
    
    public uint NameHash { get; set; }
    protected string Name { get; set; } = "";


    #region ApexSerializable

    public virtual void FromApex(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public virtual void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) (uint) VariantType);
    }
    
    #endregion


    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        throw new NotImplementedException();
    }

    public override void ToXml(XmlWriter xw)
    {
        throw new NotImplementedException();
    }

    #endregion
}