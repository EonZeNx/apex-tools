using ApexFormat.AAF.V01.Models;
using ApexFormat.SARC.V02.Models;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Utils;

namespace ApexTools.Chain.Managers;

/// <summary>
/// Passes AAF files to SARC and vice versa.
/// TODO: Do version checking.
/// </summary>
public class AafSarcChainManager : IPathProcessor
{
    public string FilePath { get; set; }

    public AafSarcChainManager(string filePath)
    {
        FilePath = filePath;
    }
    
    public void TryProcess()
    {
        if (Directory.Exists(FilePath))
        {
            FromCustomDirectoryToApex();
            return;
        }
        
        var fourCc = FileHeaderUtils.ValidCharacterCode(FilePath);
        if (fourCc is not (EFourCc.AAF or EFourCc.SARC)) throw new NotSupportedException();
        
        FromApexToCustomDirectory();
    }

    private void FromApexToCustomDirectory()
    {
        var aafV01File = new FileV01();
        var sarcV02File = new FileV02();

        using (var inBr = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
        {
            aafV01File.FromApex(inBr);
        }
        
        using var ms = new MemoryStream();
        using var br = new BinaryReader(ms);
        using var bw = new BinaryWriter(ms);
        
        aafV01File.ToCustomFile(bw);
        br.BaseStream.Seek(0, SeekOrigin.Begin);
        
        sarcV02File.FromApex(br);
        var filepathAsDirectory = Path.GetDirectoryName(FilePath) ?? @".\";
        var filepathAsFilename = Path.GetFileNameWithoutExtension(FilePath);
        var sarcV02FilePath = Path.Combine(filepathAsDirectory, filepathAsFilename);
        sarcV02File.ToCustomDirectory(sarcV02FilePath);
        
        ms.Dispose();
        br.Dispose();
        bw.Dispose();
    }

    private void FromCustomDirectoryToApex()
    {
        using var ms = new MemoryStream();
        using var br = new BinaryReader(ms);
        using var bw = new BinaryWriter(ms);
        
        var aafV01File = new FileV01();
        var sarcV02File = new FileV02();
        
        sarcV02File.FromCustomDirectory(FilePath);
        sarcV02File.ToApex(bw);
        
        br.BaseStream.Seek(0, SeekOrigin.Begin);
        aafV01File.FromCustomFile(br);
        ms.Dispose();
        br.Dispose();
        bw.Dispose();

        using (var outBw = new BinaryWriter(new FileStream($"{FilePath}.ee", FileMode.Create)))
        {
            aafV01File.ToApex(outBw); 
        }
    }
}