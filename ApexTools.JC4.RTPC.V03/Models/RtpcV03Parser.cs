using System.Xml;

namespace ApexTools.JC4.RTPC.V03.Models;

public static class RtpcV03Parser
{
    public static RtpcV03 FromApex(string targetPath)
    {
        using var br = new BinaryReader(new FileStream(targetPath, FileMode.Open));
        
        var sRtpcV03 = new RtpcV03
        {
            Extension = Path.GetExtension(targetPath)
        };

        sRtpcV03.FromApexHeader(br);
        sRtpcV03.FromApex(br);
            
        return sRtpcV03;
    }
    
    public static void ToXml(RtpcV03 rtpcV03, string targetPath)
    {
        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xw = XmlWriter.Create($"{targetPath}.xml", settings);
        rtpcV03.ToXml(xw);
    }
    
    public static RtpcV03 FromXml(string targetPath)
    {
        using var xr = XmlReader.Create(targetPath);
        
        var rtpcV03 = new RtpcV03();
        rtpcV03.FromXml(xr);

        return rtpcV03;
    }
    
    public static void ToApex(RtpcV03 rtpcV03, string targetPath)
    {
        using var bw = new BinaryWriter(new FileStream($"{targetPath}{rtpcV03.Extension}", FileMode.Create));
        rtpcV03.ToApex(bw);
    }
}