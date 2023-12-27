using ApexFormat.RTPC.V03.JC4.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.JC4.Managers;

public class Jc4RtpcV03Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public Jc4RtpcV03Manager(string path)
    {
        TargetPath = path;
    }
    
    public void TryProcess()
    {
        var fourCc = FileHeaderUtils.ValidCharacterCode(TargetPath);
        
        if (fourCc == EFourCc.Rtpc) FromApexToCustomFile();
        else if (fourCc == EFourCc.Xml) FromCustomFileToApex();
        else ApexToolsConsole.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "RTPC");
        
        using var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open));
        var rtpcV03 = new RtpcV03File
        {
            ApexExtension = Path.GetExtension(TargetPath)
        };

        rtpcV03.FromApex(br);
        ApexToolsConsole.LogProcessing(TargetPathName);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);

        var targetXmlFilePath = Path.Join(targetFilePath, $"{targetFileName}.xml");
        rtpcV03.ToXml(targetXmlFilePath);
    
        ApexToolsConsole.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "XML");
        var rtpcV03 = new RtpcV03File();
        rtpcV03.FromXml(TargetPath);
    
        ApexToolsConsole.LogProcessing(TargetPathName);
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);

        var targetApexFilePath = Path.Join(targetFilePath, $"{targetFileName}{rtpcV03.ApexExtension}");
        using var bw = new BinaryWriter(new FileStream(targetApexFilePath, FileMode.Create));
        
        rtpcV03.ToApex(bw);
    
        ApexToolsConsole.LogComplete(TargetPathName);
    }
}