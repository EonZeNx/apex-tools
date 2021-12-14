using System.Xml;

namespace EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

public abstract class XmlSerializable : IXmlSerializable
{
    public abstract string XmlName { get; }
    
    public abstract void FromXml(XmlReader xr);
    public abstract void ToXml(XmlWriter xw);
}