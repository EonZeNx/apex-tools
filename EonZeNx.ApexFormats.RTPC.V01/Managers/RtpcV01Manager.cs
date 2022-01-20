using System.Xml;
using EonZeNx.ApexFormats.RTPC.V01.Models;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Managers;

public class RtpcV01Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public RtpcV01Manager(string path)
    {
        TargetPath = path;
    }
    
    public void TryProcess()
    {
        var fourCc = FileHeaderUtils.ValidCharacterCode(TargetPath);
        
        if (fourCc == EFourCc.Rtpc) FromApexToCustomFile();
        else if (fourCc == EFourCc.Xml) FromCustomFileToApex();
        else LogUtils.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        LogUtils.LogLoading(TargetPathName, "ApexFile");
        
        var rtpcV01File = new FileV01();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            rtpcV01File.FromApex(br);
        }
        
        LogUtils.LogProcessing(TargetPathName);

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{TargetPath}.xml", settings);
        rtpcV01File.ToXml(xr);
        
        LogUtils.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        LogUtils.LogLoading(TargetPathName, "CustomFile");

        var rtpcV01File = new FileV01();
        using var xr = XmlReader.Create(TargetPath);
        rtpcV01File.FromXml(xr);
        
        LogUtils.LogProcessing(TargetPathName);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.epe", FileMode.Create));
        rtpcV01File.ToApex(bw);
        
        LogUtils.LogComplete(TargetPathName);
    }
}