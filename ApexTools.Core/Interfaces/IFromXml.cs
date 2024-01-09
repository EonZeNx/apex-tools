using System.Xml.Linq;

namespace ApexTools.Core.Interfaces;

public interface IFromXml
{
    void FromXml(XElement xe);
}