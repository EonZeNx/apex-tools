using System.Xml;

namespace ApexTools.JC4.RTPC.V03.Abstractions;

public interface IToXml
{
    string XmlName { get; }
    void ToXml(XmlWriter xw);
}

public interface IToXDoc
{
    void ToXDoc(string path);
}