using EonZeNx.ApexFormats.AAF.V01.Models;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.AAF.V01.Managers;

public class AafV01Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public AafV01Manager(string targetPath)
    {
        TargetPath = targetPath;
    }

    public void TryProcess()
    {
        var fourCc = FileHeaderUtils.ValidCharacterCode(TargetPath);
        
        if (fourCc == EFourCc.Aaf) FromApexToCustomFile();
        else if (fourCc == EFourCc.Sarc) FromCustomFileToApex();
        else LogUtils.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        LogUtils.LogLoading(TargetPathName, "ApexFile");
        var aafV01File = new FileV01();

        using (var inBinaryReader = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromApex(inBinaryReader);
        }
        
        LogUtils.LogProcessing(TargetPathName);

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{TargetPath}.sarc", FileMode.Create));
        aafV01File.ToCustomFile(outBinaryWriter);
        
        LogUtils.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        LogUtils.LogLoading(TargetPathName, "CustomFile");
        var aafV01File = new FileV01();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        aafV01File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        LogUtils.LogProcessing(TargetPathName);

        using var outFileStream = new FileStream($"{TargetPath}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        aafV01File.ToApex(outBinaryWriter);
        
        LogUtils.LogComplete(TargetPathName);
    }
}