using System.Xml;

namespace EonZeNx.ApexTools.Core.Abstractions.Serializable;

public interface IToXmlSerializable
{
    void ToXml(XmlWriter xw);
}