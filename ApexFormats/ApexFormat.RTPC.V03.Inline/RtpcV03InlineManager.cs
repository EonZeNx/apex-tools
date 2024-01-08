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
        
        if (fourCc == EFourCc.Irtpc) FromApexToCustomFile();
        else if (fourCc == EFourCc.Xml) FromCustomFileToApex();
        else ApexToolsConsole.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "ApexFile");
        
        using var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open));
        var irtpcV01File = new RtpcV03InlineFile
        {
            ApexExtension = Path.GetExtension(TargetPath)
        };
        irtpcV01File.FromApex(br);

        ApexToolsConsole.LogProcessing(TargetPathName);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);

        var targetXmlFilePath = Path.Join(targetFilePath, $"{targetFileName}.xml");
        irtpcV01File.ToXml(targetXmlFilePath);

        ApexToolsConsole.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "CustomFile");
        
        var irtpcV01File = new RtpcV03InlineFile();
        irtpcV01File.FromXml(TargetPath);
        
        ApexToolsConsole.LogProcessing(TargetPathName);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.bin", FileMode.Create));
        irtpcV01File.ToApex(bw);

        ApexToolsConsole.LogComplete(TargetPathName);
    }
}