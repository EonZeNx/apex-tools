using ApexFormat.ADF.V04.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.ADF.V04.Managers;

public class AdfV04Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public AdfV04Manager(string targetPath)
    {
        TargetPath = targetPath;
    }

    public void TryProcess()
    {
        if (Path.GetExtension(TargetPath) is ".wtunec" or ".vmodc")
        {
            FromApexToCustomFile();
        }
        else if (Path.GetExtension(TargetPath) == ".xml")
        {
            FromCustomFileToApex();
        }
        else
        {
            ConsoleUtils.Log($"Invalid path for {GetType().Name} \"{TargetPath}\"", LogType.Error);
        }
    }

    private void FromApexToCustomFile()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.Adf}", LogType.Info);
        
        var adfV04File = new FileV04();

        using (var inBinaryReader = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            adfV04File.FromApex(inBinaryReader);
        }

        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as XML", LogType.Info);
        
        using var outBinaryWriter = new BinaryWriter(new FileStream($"{TargetPath}.sarc", FileMode.Create));
        adfV04File.ToCustomFile(outBinaryWriter);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as XML", LogType.Info);
        
        var adfV04File = new FileV04();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        adfV04File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.Adf}", LogType.Info);

        using var outFileStream = new FileStream($"{TargetPath}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        adfV04File.ToApex(outBinaryWriter);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}