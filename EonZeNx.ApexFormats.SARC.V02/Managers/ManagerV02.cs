using EonZeNx.ApexFormats.SARC.V02.Models;
using EonZeNx.ApexTools.Core.Abstractions;

namespace EonZeNx.ApexFormats.SARC.V02.Managers;

public class ManagerV02 : IPathProcessor
{
    public string FilePath { get; set; }

    public ManagerV02(string filePath)
    {
        FilePath = filePath;
    }

    public void TryProcess()
    {
        if (Path.GetExtension(FilePath) == ".sarc") FromApexToCustomDirectory();
        else if (Directory.Exists(FilePath)) FromCustomDirectoryToApex();
    }

    private void FromApexToCustomDirectory()
    {
        var sarcV02File = new FileV02();
        
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
    
    private void FromCustomDirectoryToApex()
    {
        var sarcV02File = new FileV02();
        sarcV02File.FromCustomDirectory(FilePath);

        using var outputFileStream = new FileStream(Path.ChangeExtension(FilePath, "custom.sarc"), FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outputFileStream);
        
        sarcV02File.ToApex(outBinaryWriter);
        
        outputFileStream.Dispose();
        outBinaryWriter.Dispose();
    }
}