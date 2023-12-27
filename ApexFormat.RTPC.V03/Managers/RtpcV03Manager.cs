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
        
        if (fourCc == EFourCc.Rtpc) FromApexToCustomFile();
        else if (fourCc == EFourCc.Xml) FromCustomFileToApex();
        else ApexToolsLog.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        ApexToolsLog.LogLoading(TargetPathName, "ApexFile");
        
        var rtpcV01File = new FileV03();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            rtpcV01File.FromApex(br);
        }
        
        ApexToolsLog.LogProcessing(TargetPathName);

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{TargetPath}.xml", settings);
        rtpcV01File.ToXml(xr);
        
        ApexToolsLog.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        ApexToolsLog.LogLoading(TargetPathName, "CustomFile");

        var rtpcV01File = new FileV03();
        using var xr = XmlReader.Create(TargetPath);
        rtpcV01File.FromXml(xr);
        
        ApexToolsLog.LogProcessing(TargetPathName);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.epe", FileMode.Create));
        rtpcV01File.ToApex(bw);
        
        ApexToolsLog.LogComplete(TargetPathName);
    }
}