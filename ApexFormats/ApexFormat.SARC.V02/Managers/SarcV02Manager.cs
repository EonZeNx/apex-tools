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
        if (Directory.Exists(TargetPath))
        {
            FromCustomDirectoryToApex();
        }
        else if (FileHeaderUtils.ValidCharacterCode(TargetPath) == EFourCc.SARC)
        {
            FromApexToCustomDirectory();
        }
        else
        {
            ConsoleUtils.Log($"Invalid path for {GetType().Name} \"{TargetPath}\"", LogType.Error);
        }
    }

    private void FromApexToCustomDirectory()
    {
        var sarcV02File = new FileV02();
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {sarcV02File.FourCc}", LogType.Info);
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        
        sarcV02File.FromApex(inBinaryReader);
        
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as directory", LogType.Info);

        var baseDirectory = Path.GetDirectoryName(TargetPath) ?? "./";
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(TargetPath);
        
        var directoryPath = Path.Combine(baseDirectory, fileWithoutExtension);
        sarcV02File.ToCustomDirectory(directoryPath);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomDirectoryToApex()
    {
        var sarcV02File = new FileV02();
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as directory", LogType.Info);
        
        sarcV02File.FromCustomDirectory(TargetPath);

        using var outputFileStream = new FileStream(Path.ChangeExtension(TargetPath, "sarc"), FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outputFileStream);
        
        sarcV02File.ToApex(outBinaryWriter);
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {sarcV02File.FourCc}", LogType.Info);
        
        outputFileStream.Dispose();
        outBinaryWriter.Dispose();
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}