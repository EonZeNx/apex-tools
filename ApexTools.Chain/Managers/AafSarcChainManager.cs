using System.Xml;
using System.Xml.Linq;
using ApexFormat.AAF.V01.Models;
using ApexFormat.SARC.V02.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;

namespace ApexTools.Chain.Managers;

/// <summary>
/// Chains AAF files to SARC and vice versa.
/// TODO: Do version checking.
/// </summary>
public class AafSarcChainManager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public AafSarcChainManager(string targetPath)
    {
        TargetPath = targetPath;
    }
    
    public void TryProcess()
    {
        if (Directory.Exists(TargetPath) || string.Equals(Path.GetExtension(TargetPath), ".xml"))
        {
            TargetPath = Path.GetDirectoryName(TargetPath) ?? TargetPath;
            FromCustomDirectoryToApex();
        }
        else if (FileHeaderUtils.ValidCharacterCode(TargetPath) == EFourCc.AAF)
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
        var aafV01File = new AafV01File{ ApexExtension = Path.GetExtension(TargetPath) };
        var sarcV02File = new SarcV02File();

        ConsoleUtils.Log($"Loading \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
        using (var br = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            aafV01File.FromApex(br);
        }

        using (var ms = new MemoryStream())
        {
            ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
            
            using var bw = new BinaryWriter(ms);
            aafV01File.ToCustomFile(bw);

            ConsoleUtils.Log($"Chained \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);
            
            using var br = new BinaryReader(ms);
            br.Seek(0);
            sarcV02File.FromApex(br);
        }
        
        var directoryPath = Path.GetDirectoryName(TargetPath) ?? @".\";
        var filename = Path.GetFileNameWithoutExtension(TargetPath);
        var sarcV02FilePath = Path.Combine(directoryPath, filename);
        
        ConsoleUtils.Log($"Extracting \"{TargetPathName}\" files", LogType.Info);
        sarcV02File.ToCustomDirectory(sarcV02FilePath);
        
        ConsoleUtils.Log($"Saving \"{TargetPathName}\" file order", LogType.Info);
        
        var targetFilePath = Path.GetDirectoryName(TargetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(TargetPath);
        var targetXmlFilePath = Path.Join(targetFilePath, targetFileName, $"{SarcV02FileConstants.FileListName}.xml");

        var xd = new XDocument(aafV01File.ToXml());
        xd.Root!.Add(sarcV02File.ToXml());
        
        using var xw = XmlWriter.Create(targetXmlFilePath, new XmlWriterSettings{ Indent = true, IndentChars = "\t" });
        xd.Save(xw);
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }

    private void FromCustomDirectoryToApex()
    {
        var aafV01File = new AafV01File();
        var sarcV02File = new SarcV02File();

        ConsoleUtils.Log($"Loading \"{TargetPathName}\" files", LogType.Info);
        sarcV02File.FromCustomDirectory(TargetPath);
        
        using (var ms = new MemoryStream())
        {
            ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.SARC}", LogType.Info);
            
            using var bw = new BinaryWriter(ms);
            sarcV02File.ToApex(bw);
        
            ConsoleUtils.Log($"Chained \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
            
            using var br = new BinaryReader(ms);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            aafV01File.FromCustomFile(br);
        }

        {
            var xmlFileList = Path.Combine(TargetPath, $"{SarcV02FileConstants.FileListName}.xml");
            if (!File.Exists(xmlFileList))
            {
                throw new FileNotFoundException($"File list '{xmlFileList}' was not found.");
            }

            var xd = XDocument.Load(xmlFileList);
            aafV01File.ApexExtension = xd.Root!.GetAttribute("extension");
        }

        ConsoleUtils.Log($"Saving \"{TargetPathName}\" as {EFourCc.AAF}", LogType.Info);
        using (var bw = new BinaryWriter(new FileStream($"{TargetPath}{aafV01File.ApexExtension}", FileMode.Create)))
        {
            aafV01File.ToApex(bw); 
        }
        
        ConsoleUtils.Log($"Completed \"{TargetPathName}\"", LogType.Success);
    }
}