using EonZeNx.ApexFormats.AAF.V01.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.AAF.V01.Managers;

public class AafV01Manager : IPathProcessor
{
    public string Path { get; set; }

    public AafV01Manager(string path)
    {
        Path = path;
    }

    public void TryProcess()
    {
        if (System.IO.Path.GetExtension(Path) == ".ee") FromApexToCustomFile();
        else if (System.IO.Path.GetExtension(Path) == ".sarc") FromCustomFileToApex();
    }

    private void FromApexToCustomFile()
    {
        var aafV01File = new AafV01File();

        using (var inBinaryReader = new BinaryReader(new FileStream(Path, FileMode.Open)))
        {
            aafV01File.FromApex(inBinaryReader);
        }

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{Path}.sarc", FileMode.Create));
        aafV01File.ToCustomFile(outBinaryWriter);
    }
    
    private void FromCustomFileToApex()
    {
        var aafV01File = new AafV01File();
        
        using var inFileStream = new FileStream(Path, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        aafV01File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();

        using var outFileStream = new FileStream($"{Path}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        aafV01File.ToApex(outBinaryWriter);
    }
}