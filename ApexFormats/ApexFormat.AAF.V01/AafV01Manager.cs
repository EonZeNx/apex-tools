using ApexFormat.AAF.V01.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.AAF.V01;

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
        
        var aafV01File = new AafV01File
        {
            ApexExtension = Path.GetExtension(TargetPath)
        };

        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromApex(br);
        }
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetSarcFilePath = Path.Join(targetFilePath, $"{targetFileName}.sarc");
        
        using (var bw = new BinaryWriter(new FileStream(targetSarcFilePath, FileMode.Create)))
        {
            aafV01File.ToCustomFile(bw);
        }
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
    
    private void FromCustomFileToApex()
    {
        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);
        
        var aafV01File = new AafV01File();
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromCustomFile(br);
        }
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
        using (var bw = new BinaryWriter(new FileStream($"{TargetPath}{aafV01File.ApexExtension}", FileMode.Create)))
        {
            aafV01File.ToApex(bw);
        }
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}