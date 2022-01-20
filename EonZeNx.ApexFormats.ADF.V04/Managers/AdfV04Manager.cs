using EonZeNx.ApexFormats.ADF.V04.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.ADF.V04.Managers;

public class AdfV04Manager : IPathProcessor
{
    public string Path { get; set; }

    public AdfV04Manager(string path)
    {
        Path = path;
    }

    public void TryProcess()
    {
        if (System.IO.Path.GetExtension(Path) is ".wtunec" or ".vmodc") FromApexToCustomFile();
        else if (System.IO.Path.GetExtension(Path) == ".xml") FromCustomFileToApex();
    }

    private void FromApexToCustomFile()
    {
        var adfV04File = new FileV04();

        using (var inBinaryReader = new BinaryReader(new FileStream(Path, FileMode.Open)))
        {
            adfV04File.FromApex(inBinaryReader);
        }

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{Path}.sarc", FileMode.Create));
        adfV04File.ToCustomFile(outBinaryWriter);
    }
    
    private void FromCustomFileToApex()
    {
        var adfV04File = new FileV04();
        
        using var inFileStream = new FileStream(Path, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        adfV04File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();

        using var outFileStream = new FileStream($"{Path}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        adfV04File.ToApex(outBinaryWriter);
    }
}