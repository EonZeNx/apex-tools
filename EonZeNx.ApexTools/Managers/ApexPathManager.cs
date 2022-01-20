using System.Xml;
using EonZeNx.ApexFormats.ADF.V04.Managers;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Managers;
using EonZeNx.ApexFormats.IRTPC.V01.Managers;
using EonZeNx.ApexFormats.RTPC.V01.Managers;
using EonZeNx.ApexFormats.SARC.V02.Managers;
using EonZeNx.ApexFormats.SARC.V02.Models;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Exceptions;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.PassThrough.Managers;

namespace EonZeNx.ApexTools.Managers;

public class ApexPathManager
{
    private string FilePath { get; set; }

    public ApexPathManager(string filePath)
    {
        FilePath = filePath;
    }
    
    // ProcessPath function that checks if a file exists, if it does, it returns the filePath, if not, check if a directory exists, and return the filePath
    public void ProcessPath()
    {
        if (Directory.Exists(FilePath)) FilePath = Path.Combine(FilePath, FileV02.FileListName);
        var fourCc = FileHeaderUtils.ValidCharacterCode(FilePath);
        
        if (fourCc == EFourCc.Xml) fourCc = TryGetXmlFourCc(FilePath);

        IPathProcessor processor = fourCc switch
        {
            EFourCc.Aaf => new AafSarcPassThroughManager(FilePath),
            EFourCc.Rtpc => new RtpcV01Manager(FilePath),
            EFourCc.Irtpc => new IrtpcV01Manager(FilePath),
            // EFourCc.Irtpc => new IrtpcDv01Manager(FilePath),
            EFourCc.Sarc => new SarcV02Manager(FilePath),
            EFourCc.Xml => throw new NotImplementedException(),
            EFourCc.Adf => new AdfV04Manager(FilePath),
            EFourCc.Tab => throw new NotImplementedException(),
            EFourCc.Mawe => throw new NotImplementedException(),
            _ => throw new NotSupportedException()
        };
        
        processor.TryProcess();
    }
    
    private static EFourCc TryGetXmlFourCc(string path)
    {
        var xr = XmlReader.Create(path);
        xr.Read();  // Read XML declaration
        xr.Read();  // Read XML whitespace
        xr.Read();  // Read root element

        if (FileHeaderUtils.FourCcStringMap.ContainsKey(xr.Name.ToUpper()))
        {
            return FileHeaderUtils.FourCcStringMap[xr.Name.ToUpper()];
        }

        throw new MalformedXmlException("XML file is not a valid Apex file");
    }
}