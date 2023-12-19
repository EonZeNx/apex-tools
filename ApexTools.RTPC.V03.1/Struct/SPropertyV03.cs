using System.Xml;
using ApexTools.RTPC.V03._1.Abstractions;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SPropertyHeaderV03"/>
/// </summary>
public abstract class SPropertyV03 : IFromApexHeader, IFromApex, IToXml
{
    public SPropertyHeaderV03 Header;
    public abstract string XmlName { get; }

    public SPropertyV03()
    {
        Header = new SPropertyHeaderV03();
    }
    
    public SPropertyV03(SPropertyHeaderV03 header)
    {
        Header = header;
    }

    public void FromApexHeader(BinaryReader br)
    {
        Header.FromApexHeader(br);
    }

    public abstract void FromApex(BinaryReader br);
    public abstract void ToXml(XmlWriter xw);
}