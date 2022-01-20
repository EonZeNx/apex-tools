using System.Xml;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Managers;

public class IrtpcDv01Manager : IPathProcessor
{
    public string FilePath { get; set; }

    public IrtpcDv01Manager(string path)
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
        var irtpcV01File = new PassThroughFile();

        using (var br = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
        {
            irtpcV01File.FromApex(br);
        }

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{FilePath}.xml", settings);
        irtpcV01File.ToXml(xr);
    }
    
    private void FromCustomFileToApex()
    {
        var irtpcV01File = new PassThroughFile();
        using var xr = XmlReader.Create(FilePath);
        irtpcV01File.FromXml(xr);
        
        using var bw = new BinaryWriter(new FileStream($"{FilePath}.bin", FileMode.Create));
        irtpcV01File.ToApex(bw);
    }
}