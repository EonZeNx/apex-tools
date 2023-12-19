using System.Xml;

namespace ApexTools.RTPC.V03._1.Abstractions;

public interface IToXml
{
    string XmlName { get; }
    void ToXml(XmlWriter xw);
}