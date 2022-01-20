using System.Xml;
using EonZeNx.ApexFormats.IRTPC.V01.Models;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Managers;

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
        else LogUtils.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        LogUtils.LogLoading(TargetPathName, "ApexFile");
        
        var irtpcV01File = new FileV01();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            irtpcV01File.FromApex(br);
        }
        
        LogUtils.LogProcessing(TargetPathName);

        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        using var xr = XmlWriter.Create($"{TargetPath}.xml", settings);
        irtpcV01File.ToXml(xr);

        LogUtils.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        LogUtils.LogLoading(TargetPathName, "CustomFile");
        
        var irtpcV01File = new FileV01();
        using var xr = XmlReader.Create(TargetPath);
        irtpcV01File.FromXml(xr);
        
        LogUtils.LogProcessing(TargetPathName);
        
        using var bw = new BinaryWriter(new FileStream($"{TargetPath}.bin", FileMode.Create));
        irtpcV01File.ToApex(bw);

        LogUtils.LogComplete(TargetPathName);
    }
}