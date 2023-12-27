using System.Xml;

namespace ApexTools.Core.Abstractions.Serializable;

public interface IToXmlSerializable
{
    void ToXml(XmlWriter xw);
}