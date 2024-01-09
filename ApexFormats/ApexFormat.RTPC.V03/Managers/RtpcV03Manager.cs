using System.Xml;
using ApexFormat.RTPC.V03.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Managers;

public class RtpcV03Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public RtpcV03Manager(string path)
    {
        TargetPath = path;
    }
    
    public void TryProcess()
    {
        var fourCc = FileHeaderUtils.ValidCharacterCode(TargetPath);

        if (fourCc == EFourCc.RTPC)
        {
            FromApexToCustomFile();
        }
        else if (fourCc == EFourCc.XML)
        {
            FromCustomFileToApex();
        }
        else
        {
            ConsoleUtils.Log($"Invalid path for {GetType().Name} \"{TargetPath}\"", LogType.Error);
        }
    }

    private void FromApexToCustomFile()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.RTPC}", LogType.Info);
        
        var rtpcV01File = new FileV03();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            rtpcV01File.FromApex(br);
        }
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as XML", LogType.Info);

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{TargetPath}.xml", settings);
        rtpcV01File.ToXml(xr);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as XML", LogType.Info);

        var rtpcV01File = new FileV03();
        using var xr = XmlReader.Create(TargetPath);
        rtpcV01File.FromXml(xr);
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.RTPC} flat", LogType.Info);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.epe", FileMode.Create));
        rtpcV01File.ToApex(bw);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}