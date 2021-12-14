using System.Xml;

namespace EonZeNx.ApexTools.Core.Abstractions.Serializable;

public interface IFromXmlSerializable
{
    void FromXml(XmlReader xr);
}