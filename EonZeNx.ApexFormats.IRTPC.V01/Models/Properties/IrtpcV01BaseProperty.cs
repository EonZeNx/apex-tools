using System.Xml;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties;

public class IrtpcV01BaseProperty : XmlSerializable, IApexSerializable
{
    public override string XmlName => "BaseProperty";
    
    protected virtual EVariantType VariantType { get; set; }
    protected long Offset { get; set; }
    protected int NameHash { get; set; }
    protected string Name { get; set; } = "";


    public IrtpcV01BaseProperty() { }
    
    public IrtpcV01BaseProperty(IrtpcV01LoadProperty loadProperty)
    {
        Offset = loadProperty.Offset;
        NameHash = loadProperty.NameHash;
    }


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
    
    
    #region CustomSerializable

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