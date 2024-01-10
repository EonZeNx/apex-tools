using System.Xml.Linq;

namespace ApexTools.Core.Interfaces;

public interface IToXml
{
    public static abstract string XmlName { get; }
    
    XElement ToXml();
}