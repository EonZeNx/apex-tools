using System.Xml;
using ApexFormat.ADF.V04.Managers;
using ApexFormat.RTPC.V03.Flat.Managers;
using ApexFormat.RTPC.V03.Inline;
using ApexFormat.RTPC.V03.Managers;
using ApexFormat.SARC.V02.Managers;
using ApexFormat.SARC.V02.Models;
using ApexTools.Chain.Managers;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Config;
using ApexTools.Core.Exceptions;
using ApexTools.Core.Utils;

namespace ApexTools.Managers;

public class ApexPathManager
{
    private string FilePath { get; set; }

    public ApexPathManager(string filePath)
    {
        FilePath = filePath;
    }
    
    // Checks if a file exists, if it does return the filePath, if not check if a directory exists, and return the filePath
    public void ProcessPath()
    {
        if (Directory.Exists(FilePath)) FilePath = Path.Combine(FilePath, FileV02.FileListName);
        var fourCc = FileHeaderUtils.ValidCharacterCode(FilePath);

        if (fourCc == EFourCc.Xml)
        {
            fourCc = TryGetXmlFourCc(FilePath);
        }

        IPathProcessor processor = fourCc switch
        {
            EFourCc.Aaf => new AafSarcChainManager(FilePath),
            EFourCc.Rtpc => Settings.RtpcPreferFlat.Value ? new RtpcV03FlatManager(FilePath) : new RtpcV03Manager(FilePath),
            EFourCc.Irtpc => new RtpcV03InlineManager(FilePath),
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

        if (!FileHeaderUtils.FourCcStringMap.TryGetValue(xr.Name.ToUpper(), out var value))
        {
            throw new MalformedXmlException("XML file is not a valid Apex file");
            
        }

        if (value == EFourCc.Rtpc)
        {
            // Could be inline
            var inlineValue = xr.GetAttribute("Inline");
            if (!string.IsNullOrEmpty(inlineValue))
            {
                value = EFourCc.Irtpc;
            }
        }
        
        return value;
    }
}