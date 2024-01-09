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

        if (fourCc == EFourCc.Rtpc)
        {
            FromApexToCustomFile();
        }
        else if (fourCc == EFourCc.Xml)
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
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.Rtpc} flat", LogType.Info);
        
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
        rtpcV03.ToXml(targetXmlFilePath);
    
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as XML", LogType.Info);
        
        var rtpcV03 = new RtpcV03File();
        rtpcV03.FromXml(TargetPath);
    
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.Rtpc} flat", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);

        var targetApexFilePath = Path.Join(targetFilePath, $"{targetFileName}{rtpcV03.ApexExtension}");
        using var bw = new BinaryWriter(new FileStream(targetApexFilePath, FileMode.Create));
        
        rtpcV03.ToApex(bw);
    
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}