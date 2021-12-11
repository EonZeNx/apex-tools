using EonZeNx.ApexFormats.AAF.V01.Models;
using EonZeNx.ApexFormats.SARC.V02.Models;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexTools.PassThrough.Managers;

/// <summary>
/// Passes AAF files to SARC and vice versa.
/// TODO: Do version checking.
/// </summary>
public class AafSarcPassThroughManager : IPathProcessor
{
    public string FilePath { get; set; }

    public AafSarcPassThroughManager(string filePath)
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
        if (fourCc is EFourCc.Aaf or EFourCc.Sarc)
        {
            FromApexToCustomDirectory();
            return;
        }

        throw new NotSupportedException();
    }

    private void FromApexToCustomDirectory()
    {
        var aafV01File = new AafV01File();
        var sarcV02File = new SarcV02File();

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
        
        var aafV01File = new AafV01File();
        var sarcV02File = new SarcV02File();
        
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