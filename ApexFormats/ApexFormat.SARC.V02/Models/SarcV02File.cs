using System.Xml;
using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;

namespace ApexFormat.SARC.V02.Models;

public static class SarcV02FileConstants
{
    public const string FileListName = "@files";
    public static readonly string[] IgnoreExtensions = {
        ".xml",
        ".bak"
    };
}

/// <summary>
/// Structure:
/// <br/>Header Length - <see cref="uint"/>
/// <br/>FourCC - <see cref="EFourCc"/>
/// <br/>Version - <see cref="uint"/>
/// <br/>Data offset - <see cref="uint"/>
/// <br/>Entries array - <see cref="SarcV02Entry"/>
/// </summary>
public class SarcV02File : IApexFile, IApexSerializable, ICustomDirectorySerializable
{
    public EFourCc FourCc => EFourCc.SARC;
    public uint Version => 0x02;
    
    
    public uint HeaderLength { get; set; }
    public uint DataOffset { get; set; }
    public SarcV02Entry[] Entries { get; set; } = Array.Empty<SarcV02Entry>();


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        HeaderLength = br.ReadUInt32();
        
        var readFourCc = br.ReadBigUInt32();
        if (!FileHeaderUtils.IsCharacterCode(readFourCc, FourCc)) throw new IOException($"Character code was not valid (Expected '{(uint) FourCc}' got '{readFourCc}')");
        
        var readVersion = br.ReadUInt32();
        if (readVersion != Version) throw new IOException($"Version was not valid (Expected '{Version}' got '{readVersion}')");
        
        DataOffset = br.ReadUInt32();
        
        var entries = new List<SarcV02Entry>();
        while (br.Position() < 4 + DataOffset)
        {
            var entry = new SarcV02Entry();
            entry.FromApex(br);
            entries.Add(entry);
        }

        Entries = entries.ToArray();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write((uint) 4);
        bw.Write(FourCc.ToBigEndian());
        bw.Write(Version);

        // Include size of data offset in calculation (+4)
        var dataOffset = (uint) Entries.Sum(entry => entry.HeaderSize) + 4;
        bw.Write(dataOffset);
            
        bw.Seek((int) dataOffset, SeekOrigin.Current);

        foreach (var entry in Entries)
        {
            entry.ToApexDeferred(bw);
        }

        bw.Seek(16, SeekOrigin.Begin);
        foreach (var entry in Entries)
        {
            entry.ToApex(bw);
        }
    }

    #endregion


    #region Custom Directory Serializable

    public void FromCustomDirectory(string basePath)
    {
        var xmlFileList = Path.Combine(basePath, $"{SarcV02FileConstants.FileListName}.xml");
        if (!File.Exists(xmlFileList)) throw new FileNotFoundException($"File list '{xmlFileList}' was not found.");
        
        using var xr = XmlReader.Create(xmlFileList);
        XmlLoadReferences(xr, basePath);
    }

    public void ToCustomDirectory(string basePath)
    {
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        var xmlFileList = Path.Combine(basePath, $"{SarcV02FileConstants.FileListName}.xml");
        
        var settings = new XmlWriterSettings{Indent = true, IndentChars = "\t"};
        using var xw = XmlWriter.Create(xmlFileList, settings);
        
        xw.WriteStartElement("sarc");
        XmlWriteIgnores(xw);
        XmlWriteEntries(xw, basePath);
        xw.WriteEndElement();
    }

    #endregion


    #region XmlUtils

    #region XmlWriter Utils

    private void XmlWriteEntries(XmlWriter xw, string directoryPath)
    {
        xw.WriteStartElement("files");
            
        foreach (var entry in Entries)
        {
            entry.XmlWrite(xw);
            entry.ToCustomFile(directoryPath);
        }
            
        xw.WriteEndElement();
    }
    
    private static void XmlWriteIgnores(XmlWriter xw)
    {
        // Ignore these extensions when deserializing the folder
        xw.WriteStartElement("ignore");
        foreach (var ignoreExtension in SarcV02FileConstants.IgnoreExtensions)
        {
            xw.WriteStartElement("extension");
            xw.WriteAttributeString("value", ignoreExtension);
            xw.WriteEndElement();
        }
        xw.WriteEndElement();
    }

    #endregion

    #region XmlReader Utils

    private void XmlLoadReferences(XmlReader xr, string basePath)
    {
        if (Path.HasExtension(basePath)) basePath = Path.GetDirectoryName(basePath) ?? basePath;
            
        var entries = new List<SarcV02Entry>();
        xr.ReadToDescendant("Entry");

        do
        {
            if (xr.NodeType != XmlNodeType.Element) continue;
            if (xr.Name != "Entry") break;

            var entry = new SarcV02Entry(xr, basePath);
            entries.Add(entry);
        } while (xr.ReadToNextSibling("Entry"));
        xr.ReadEndElement();
            
        Entries = entries.ToArray();
    }

    #endregion

    #endregion
}