using System.Xml;
using ApexFormat.IRTPC.V01.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Managers;

public class IrtpcV01Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public IrtpcV01Manager(string path)
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
        
        var irtpcV01File = new FileV01();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            irtpcV01File.FromApex(br);
        }
        
        ApexToolsConsole.LogProcessing(TargetPathName);

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{TargetPath}.xml", settings);
        irtpcV01File.ToXml(xr);

        ApexToolsConsole.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "CustomFile");
        
        var irtpcV01File = new FileV01();
        using var xr = XmlReader.Create(TargetPath);
        irtpcV01File.FromXml(xr);
        
        ApexToolsConsole.LogProcessing(TargetPathName);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.bin", FileMode.Create));
        irtpcV01File.ToApex(bw);

        ApexToolsConsole.LogComplete(TargetPathName);
    }
}