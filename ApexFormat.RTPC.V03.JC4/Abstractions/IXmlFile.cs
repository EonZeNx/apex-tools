namespace ApexFormat.RTPC.V03.JC4.Abstractions;

public interface IXmlFile : IFromXml, IToXml
{
    string XmlExtension { get; }
    static abstract string XmlName { get; }
}