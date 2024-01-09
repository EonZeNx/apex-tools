using System.Xml.Linq;

namespace ApexTools.Core.Interfaces;

public interface IToXml
{
    XElement ToXml();
}