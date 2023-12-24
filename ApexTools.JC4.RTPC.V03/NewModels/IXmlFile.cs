using ApexTools.JC4.RTPC.V03.Abstractions;

namespace ApexTools.JC4.RTPC.V03.NewModels;

public interface IXmlFile : IFromXml, IToXml
{
    public string XmlExtension { get; }
}