using EonZeNx.ApexFormats.AAF.V01.Managers;
using EonZeNx.ApexFormats.IRTPC.V01.Managers;
using EonZeNx.ApexFormats.RTPC.V01.Managers;
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
        if (ext is ".ee" or ".sarc")
        {
            processor = new AafSarcPassThroughManager(Path);
        }
        else if (ext is ".bin")
        {
            processor = new IrtpcV01Manager(Path);
        }
        else if (ext is ".epe")
        {
            processor = new RtpcV01Manager(Path);
        }
        else if (ext is ".xml")
        {
            processor = new RtpcV01Manager(Path);
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
        using var reader = new BinaryReader(File.Open(Path, FileMode.Open));
        var fourCharacterCode = reader.ReadChars(4).ToString();
        
        return fourCharacterCode;
    }
}