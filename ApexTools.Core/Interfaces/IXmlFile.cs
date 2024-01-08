namespace ApexTools.Core.Interfaces;

public interface IXmlFile : IFromXml, IToXml
{
    string XmlExtension { get; }
    static abstract string XmlName { get; }
}