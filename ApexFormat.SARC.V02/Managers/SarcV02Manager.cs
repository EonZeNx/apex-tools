using ApexFormat.SARC.V02.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.SARC.V02.Managers;

public class SarcV02Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public SarcV02Manager(string targetPath)
    {
        TargetPath = targetPath;
    }

    public void TryProcess()
    {
        if (Directory.Exists(TargetPath)) FromCustomDirectoryToApex();
        else if (FileHeaderUtils.ValidCharacterCode(TargetPath) == EFourCc.Sarc) FromApexToCustomDirectory();
        else ApexToolsLog.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomDirectory()
    {
        ApexToolsLog.LogLoading(TargetPathName, "ApexFile");
        var sarcV02File = new FileV02();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        
        sarcV02File.FromApex(inBinaryReader);
        
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        ApexToolsLog.LogProcessing(TargetPathName);

        var baseDirectory = Path.GetDirectoryName(TargetPath) ?? "./";
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(TargetPath);
        
        var directoryPath = Path.Combine(baseDirectory, fileWithoutExtension);
        sarcV02File.ToCustomDirectory(directoryPath);
        
        ApexToolsLog.LogComplete(TargetPathName);
    }
    
    private void FromCustomDirectoryToApex()
    {
        ApexToolsLog.LogLoading(TargetPathName, "CustomFile");
        var sarcV02File = new FileV02();
        sarcV02File.FromCustomDirectory(TargetPath);

        using var outputFileStream = new FileStream(Path.ChangeExtension(TargetPath, "sarc"), FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outputFileStream);
        
        sarcV02File.ToApex(outBinaryWriter);
        
        ApexToolsLog.LogProcessing(TargetPathName);
        
        outputFileStream.Dispose();
        outBinaryWriter.Dispose();
        
        ApexToolsLog.LogComplete(TargetPathName);
    }
}