using System.Xml;

namespace ApexTools.Core.Abstractions.Serializable;

public interface IFromXmlSerializable
{
    void FromXml(XmlReader xr);
}