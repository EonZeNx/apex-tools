using System.Xml;

namespace ApexTools.JC4.RTPC.V03.NewModels;

public static class DataRtpcV03Parser
{
    public static RtpcV03File FromApex(string targetPath)
    {
        using var br = new BinaryReader(new FileStream(targetPath, FileMode.Open));
        
        var sRtpcV03 = new RtpcV03File
        {
            ApexExtension = Path.GetExtension(targetPath)
        };

        sRtpcV03.FromApex(br);
            
        return sRtpcV03;
    }
    
    public static void ToXml(RtpcV03File rtpcV03, string targetPath)
    {
        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xw = XmlWriter.Create($"{targetPath}.xml", settings);
        rtpcV03.ToXml(xw);
    }
    
    public static RtpcV03File FromXml(string targetPath)
    {
        using var xr = XmlReader.Create(targetPath);
        
        var rtpcV03 = new RtpcV03File();
        rtpcV03.FromXml(xr);

        return rtpcV03;
    }
    
    public static void ToApex(RtpcV03File rtpcV03, string targetPath)
    {
        using var bw = new BinaryWriter(new FileStream($"{targetPath}{rtpcV03.ApexExtension}", FileMode.Create));
        rtpcV03.ToApex(bw);
    }
}