using System.Xml;
using System.Xml.Schema;
using ApexFormat.AAF.V01;
using ApexFormat.ADF.V04.Managers;
using ApexFormat.RTPC.V03;
using ApexFormat.RTPC.V03.Flat;
using ApexFormat.RTPC.V03.Inline;
using ApexFormat.SARC.V02;
using ApexFormat.SARC.V02.Models;
using ApexTools.Chain.Managers;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Config;
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
        if (Directory.Exists(FilePath)) FilePath = Path.Combine(FilePath, SarcV02FileConstants.FileListName);
        var fourCc = FileHeaderUtils.ValidCharacterCode(FilePath);

        if (fourCc == EFourCc.XML)
        {
            fourCc = TryGetXmlFourCc(FilePath);
        }

        IPathProcessor processor = fourCc switch
        {
            EFourCc.AAF => new AafV01Manager(FilePath),
            EFourCc.RTPC => Settings.RtpcPreferFlat.Value ? new RtpcV03FlatManager(FilePath) : new RtpcV03Manager(FilePath),
            EFourCc.IRTPC => new RtpcV03InlineManager(FilePath),
            // EFourCc.SARC => new SarcV02Manager(FilePath),
            EFourCc.SARC => new AafV01Manager(FilePath),
            EFourCc.XML => throw new NotImplementedException(),
            EFourCc.ADF => new AdfV04Manager(FilePath),
            EFourCc.TAB => throw new NotImplementedException(),
            EFourCc.MAWE => throw new NotImplementedException(),
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
            throw new XmlSchemaException("XML file is not a valid Apex file");
            
        }

        if (value == EFourCc.RTPC)
        {
            // Could be inline
            var inlineValue = xr.GetAttribute("Inline");
            if (!string.IsNullOrEmpty(inlineValue))
            {
                value = EFourCc.IRTPC;
            }
        }
        
        return value;
    }
}