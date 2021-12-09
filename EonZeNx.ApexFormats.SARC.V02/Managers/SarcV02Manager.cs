using EonZeNx.ApexFormats.SARC.V02.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.SARC.V02.Managers;

public class SarcV02Manager : IPathProcessor
{
    public string FilePath { get; set; }

    public SarcV02Manager(string filePath)
    {
        FilePath = filePath;
    }

    public void TryProcess()
    {
        if (Path.GetExtension(FilePath) == ".sarc") FromApexToCustomFile();
        else if (Directory.Exists(FilePath)) FromCustomFileToApex();
    }

    private void FromApexToCustomFile()
    {
        var sarcV02File = new SarcV02File();
        
        using var inFileStream = new FileStream(FilePath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        
        sarcV02File.FromApex(inBinaryReader);
        
        inBinaryReader.Dispose();
        inFileStream.Dispose();

        var baseDirectory = Path.GetDirectoryName(FilePath) ?? "./";
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
        
        var directoryPath = Path.Combine(baseDirectory, fileWithoutExtension);
        sarcV02File.ToCustomDirectory(directoryPath);
    }
    
    private void FromCustomFileToApex()
    {
        var sarcV02File = new SarcV02File();
        sarcV02File.FromCustomDirectory(FilePath);

        using var outputFileStream = new FileStream(Path.ChangeExtension(FilePath, "custom.sarc"), FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outputFileStream);
        
        sarcV02File.ToApex(outBinaryWriter);
        
        outputFileStream.Dispose();
        outBinaryWriter.Dispose();
    }
}