using System.Xml;
using ApexTools.JC4.RTPC.V03.NewModels.Data;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.NewModels;

public class RtpcV03File : IApexFile, IXmlFile
{
    public RtpcV03Header Header;
    public RtpcV03Container Container;
    
    public string ApexExtension { get; set; } = ".rtpc";
    public string XmlName => "RTPC";
    public string XmlExtension => ".xml";
    
    public void FromApex(BinaryReader br)
    {
        br.Seek(0);
        
        Header = br.ReadRtpcV03Header();
        var containerHeader = br.ReadRtpcV03ContainerHeader();
        Container = br.ReadRtpcV03Container(containerHeader);
        
        
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Seek(0, SeekOrigin.Begin);
        
        bw.Write(Header);
        bw.Write(Container);
    }

    public void FromXml(XmlReader xr)
    {
        throw new NotImplementedException();
    }
    
    public void ToXml(XmlWriter xw)
    {
        throw new NotImplementedException();
    }
}