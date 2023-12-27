using ApexTools.JC4.RTPC.V03.Abstractions;

namespace ApexTools.JC4.RTPC.V03.NewModels;

public interface IXmlFile : IFromXml, IToXml
{
    string XmlExtension { get; }
}

public interface IXDocFile : IFromXDoc, IToXDoc
{
    string XmlExtension { get; }
    static abstract string XmlName { get; }
}