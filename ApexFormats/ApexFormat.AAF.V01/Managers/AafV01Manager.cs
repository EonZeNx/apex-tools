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

        if (fourCc == EFourCc.AAF)
        {
            FromApexToCustomFile();
        }
        else if (fourCc == EFourCc.SARC)
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
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
        
        var aafV01File = new FileV01();

        using (var inBinaryReader = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromApex(inBinaryReader);
        }
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{TargetPath}.sarc", FileMode.Create));
        aafV01File.ToCustomFile(outBinaryWriter);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);
        var aafV01File = new FileV01();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        aafV01File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);

        using var outFileStream = new FileStream($"{TargetPath}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        aafV01File.ToApex(outBinaryWriter);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}