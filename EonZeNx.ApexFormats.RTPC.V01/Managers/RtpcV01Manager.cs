using System.Xml;
using EonZeNx.ApexFormats.RTPC.V01.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.RTPC.V01.Managers;

public class RtpcV01Manager : IPathProcessor
{
    public string FilePath { get; set; }

    public RtpcV01Manager(string path)
    {
        FilePath = path;
    }
    
    public void TryProcess()
    {
        if (Path.GetExtension(FilePath) == ".epe") FromApexToCustomFile();
        else if (Path.GetExtension(FilePath) == ".xml") FromCustomFileToApex();
    }

    private void FromApexToCustomFile()
    {
        var rtpcV01File = new RtpcV01File();

        using (var br = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
        {
            rtpcV01File.FromApex(br);
        }

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{FilePath}.xml", settings);
        rtpcV01File.ToXml(xr);
    }
    
    private void FromCustomFileToApex()
    {
        var rtpcV01File = new RtpcV01File();
        using var xr = XmlReader.Create(FilePath);
        rtpcV01File.FromXml(xr);
        
        using var bw = new BinaryWriter(new FileStream($"{FilePath}.epe", FileMode.Create));
        rtpcV01File.ToApex(bw);
    }
}