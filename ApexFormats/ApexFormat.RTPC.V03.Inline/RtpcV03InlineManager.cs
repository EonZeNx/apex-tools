using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Inline.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Inline;

public class RtpcV03InlineManager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);
    
    public RtpcV03InlineManager(string path)
    {
        TargetPath = path;
    }
    
    public void TryProcess()
    {
        var fourCc = FileHeaderUtils.ValidCharacterCode(TargetPath);

        if (fourCc == EFourCc.IRTPC)
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
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.RTPC} inline", LogType.Info);
        
        using var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open));
        var rtpcV03InlineFile = new RtpcV03InlineFile
        {
            ApexExtension = Path.GetExtension(TargetPath)
        };
        rtpcV03InlineFile.FromApex(br);

        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as XML", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetXmlFilePath = Path.Join(targetFilePath, $"{targetFileName}.xml");
        
        var xd = new XDocument(rtpcV03InlineFile.ToXml());
        using var xw = XmlWriter.Create(targetXmlFilePath, new XmlWriterSettings{ Indent = true, IndentChars = "\t" });
        xd.Save(xw);

        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as XML", LogType.Info);
        
        var xd = XDocument.Load(TargetPathName);
        if (xd.Root is null) throw new XmlSchemaException("Root element is invalid");
        
        var rtpcV03InlineFile = new RtpcV03InlineFile();
        rtpcV03InlineFile.FromXml(xd.Root);
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.RTPC} inline", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetApexFilePath = Path.Join(targetFilePath, $"{targetFileName}{rtpcV03InlineFile.ApexExtension}");
        
        using var bw = new BinaryWriter(new FileStream(targetApexFilePath, FileMode.Create));
        rtpcV03InlineFile.ToApex(bw);

        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}