using System.Xml.Linq;

namespace ApexFormat.RTPC.V03.Inline.Interfaces;

public interface IFromXElement
{
    public void FromXElement(XElement xe);
}