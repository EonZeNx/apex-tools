using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Flat.Managers;

public class RtpcV03FlatManager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public RtpcV03FlatManager(string path)
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
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.RTPC} flat", LogType.Info);
        
        using var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open));
        var rtpcV03 = new RtpcV03File
        {
            ApexExtension = Path.GetExtension(TargetPath)
        };
        rtpcV03.FromApex(br);
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as XML", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetXmlFilePath = Path.Join(targetFilePath, $"{targetFileName}.xml");
        
        var xd = new XDocument(rtpcV03.ToXml());
        using var xw = XmlWriter.Create(targetXmlFilePath, new XmlWriterSettings{ Indent = true, IndentChars = "\t" });
        xd.Save(xw);
    
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as XML", LogType.Info);
        
        var xd = XDocument.Load(TargetPathName);
        if (xd.Root is null) throw new XmlSchemaException("Root element is invalid");
        
        var rtpcV03 = new RtpcV03File();
        rtpcV03.FromXml(xd.Root);
    
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.RTPC.ToString().ToUpper()} flat", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetApexFilePath = Path.Join(targetFilePath, $"{targetFileName}{rtpcV03.ApexExtension}");
        
        using var bw = new BinaryWriter(new FileStream(targetApexFilePath, FileMode.Create));
        rtpcV03.ToApex(bw);
    
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}