namespace ApexFormat.RTPC.V03.Flat.Abstractions;

public interface IXmlFile : IFromXml, IToXml
{
    string XmlExtension { get; }
    static abstract string XmlName { get; }
}