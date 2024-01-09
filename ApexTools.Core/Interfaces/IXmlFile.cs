namespace ApexTools.Core.Interfaces;

public interface IXmlFile : IFromXml, IToXml
{
    public static abstract string XmlName { get; }
}