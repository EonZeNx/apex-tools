using System.Xml;
using ApexTools.JC4.RTPC.V03.Models;

namespace ApexTools.JC4.RTPC.V03.Abstractions;

/// <summary>
/// Format:<br/>
/// Header - <see cref="PropertyHeaderV03"/>
/// </summary>
public abstract class APropertyV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApex
{
    public PropertyHeaderV03 Header;
    public abstract string XmlName { get; }

    public APropertyV03()
    {
        Header = new PropertyHeaderV03();
    }
    
    public APropertyV03(PropertyHeaderV03 header)
    {
        Header = header;
    }

    public void FromApexHeader(BinaryReader br)
    {
        Header.FromApexHeader(br);
    }

    public abstract void FromApex(BinaryReader br);
    public abstract void ToXml(XmlWriter xw);
    public abstract void FromXml(XmlReader xr);
    public abstract void ToApex(BinaryWriter bw);
}