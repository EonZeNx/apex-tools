using EonZeNx.ApexFormats.AAF.V01.Managers;
using EonZeNx.ApexFormats.SARC.V02.Managers;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.PassThrough.Managers;

namespace EonZeNx.ApexTools.Managers;

public class ApexPathManager
{
    private string Path { get; }

    public ApexPathManager(string path)
    {
        Path = path;
    }
    
    // ProcessPath function that checks if a file exists, if it does, it returns the path, if not, check if a directory exists, and return the path
    public void ProcessPath()
    {
        IPathProcessor processor;
        if (Directory.Exists(Path))
        {
            processor = new AafSarcPassThroughManager(Path);
            processor.TryProcess();
            return;
        }
        
        if (!File.Exists(Path)) return;
        
        var ext = System.IO.Path.GetExtension(Path);
        if (ext is not (".ee" or ".sarc")) return;

        if (ext is ".ee")
        {
            processor = new AafSarcPassThroughManager(Path);
        }
        else if (ext is ".sarc")
        {
            processor = new SarcV02Manager(Path);
        }
        else
        {
            throw new IOException("Unknown file extension");
        }
        
        processor.TryProcess();
    }
    
    // ProcessPath function that opens the file, reads the four character code, and returns it
    public string GetFileCharacterCode()
    {
        var fourCharacterCode = "";
        using var reader = new BinaryReader(File.Open(Path, FileMode.Open));
        fourCharacterCode = reader.ReadChars(4).ToString();
        
        return fourCharacterCode;
    }
}