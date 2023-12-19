using System.Xml;

namespace ApexTools.RTPC.V03._1.Struct;

public static class RtpcV03Parser
{
    public static SRtpcV03 FromApex(BinaryReader br)
    {
        var sRtpcV03 = new SRtpcV03();
        sRtpcV03.FromApexHeader(br);
        sRtpcV03.FromApex(br);

        return sRtpcV03;
    }
    
    public static void ToXml(SRtpcV03 rtpcV03, string targetPath)
    {
        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xw = XmlWriter.Create($"{targetPath}.xml", settings);
        rtpcV03.ToXml(xw);
    }
}