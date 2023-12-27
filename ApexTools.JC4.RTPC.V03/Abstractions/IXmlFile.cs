namespace ApexTools.JC4.RTPC.V03.Abstractions;

public interface IXmlFile : IFromXml, IToXml
{
    string XmlExtension { get; }
    static abstract string XmlName { get; }
}