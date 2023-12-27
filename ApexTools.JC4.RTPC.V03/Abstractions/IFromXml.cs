using System.Xml;

namespace ApexTools.JC4.RTPC.V03.Abstractions;

public interface IFromXml
{
    void FromXml(XmlReader xr);
}

public interface IFromXDoc
{
    void FromXDoc(string path);
}