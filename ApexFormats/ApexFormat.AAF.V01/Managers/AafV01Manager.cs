using ApexFormat.AAF.V01.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.AAF.V01.Managers;

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
        else ApexToolsConsole.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "ApexFile");
        var aafV01File = new FileV01();

        using (var inBinaryReader = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromApex(inBinaryReader);
        }
        
        ApexToolsConsole.LogProcessing(TargetPathName);

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{TargetPath}.sarc", FileMode.Create));
        aafV01File.ToCustomFile(outBinaryWriter);
        
        ApexToolsConsole.LogComplete(TargetPathName);
    }
    
    private void FromCustomFileToApex()
    {
        ApexToolsConsole.LogLoading(TargetPathName, "CustomFile");
        var aafV01File = new FileV01();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        aafV01File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        ApexToolsConsole.LogProcessing(TargetPathName);

        using var outFileStream = new FileStream($"{TargetPath}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        aafV01File.ToApex(outBinaryWriter);
        
        ApexToolsConsole.LogComplete(TargetPathName);
    }
}