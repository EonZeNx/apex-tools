using System.Xml.Linq;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Extensions;
using ApexTools.Core.Interfaces;

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
/// <br/>Header - <see cref="SarcV02Header"/>
/// <br/>Entries array - <see cref="SarcV02Entry"/>
/// </summary>
public class SarcV02File : IApexFile, ICustomDirectorySerializable, IXmlFile
{
    public SarcV02Header Header = new();
    public SarcV02Entry[] Entries { get; set; } = Array.Empty<SarcV02Entry>();

    public string ApexExtension { get; set; } = ".sarc";
    public static string XmlName => "sarc";


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        Header = br.ReadSarcV02Header();
        
        var entryHeaders = new List<SarcV02EntryHeader>();
        while (br.Position() < 4 + Header.DataOffset)
        {
            var entryHeader = br.ReadSarcV02EntryHeader();
            entryHeaders.Add(entryHeader);
        }

        Entries = entryHeaders
            .Select(header => new SarcV02Entry { Header = header })
            .ToArray();
        foreach (ref var fileEntry in Entries.AsSpan())
        {
            fileEntry.FromApex(br);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(Header);
        
        bw.Seek((int) Header.DataOffset, SeekOrigin.Current);
        
        foreach (ref var entry in Entries.AsSpan())
        {
            entry.ToApexDeferred(bw);
        }
        
        bw.Seek((int) SarcV02Header.SizeOf(), SeekOrigin.Begin);
        foreach (ref var entry in Entries.AsSpan())
        {
            entry.ToApex(bw);
        }
    }

    #endregion


    #region Custom Directory Serializable

    public void FromCustomDirectory(string basePath)
    {
        var xmlFileList = Path.Combine(basePath, $"{SarcV02FileConstants.FileListName}.xml");
        if (!File.Exists(xmlFileList))
        {
            throw new FileNotFoundException($"File list '{xmlFileList}' was not found.");
        }

        var xd = XDocument.Load(xmlFileList);
        var fileEntryElements = xd.Descendants(SarcV02Entry.XmlName).ToList();

        Entries = fileEntryElements
            .Select(element => new SarcV02Entry(element.ReadSarcV02EntryHeader()))
            .ToArray();

        foreach (ref var entry in Entries.AsSpan())
        {
            entry.FromCustomFile(basePath);
        }

        Header = new SarcV02Header
        { // Data offset value is relative to position of data offset before writing it
            DataOffset = SarcV02Header.SizeOf() - 4 + (uint) Entries.Sum(entry => entry.SizeOf())
        };
    }

    public void ToCustomDirectory(string basePath)
    {
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        foreach (ref var entry in Entries.AsSpan())
        {
            entry.ToCustomFile(basePath);
        }
    }

    #endregion


    #region XmlSerializable

    public void FromXml(XElement xe)
    {
        throw new NotImplementedException();
    }

    public XElement ToXml()
    {
        var xe = new XElement(XmlName);
        xe.SetAttributeValue("extension", ApexExtension);
        
        // var ixe = new XElement("ignores");
        // foreach (var ignoreExtension in SarcV02FileConstants.IgnoreExtensions)
        // {
        //     var ieXe = new XElement("ignore");
        //     ieXe.SetAttributeValue("type", "extension");
        //     ieXe.SetAttributeValue("value", ignoreExtension);
        //     
        //     ixe.Add(ieXe);
        // }
        // xe.Add(ixe);
        
        foreach (ref var entry in Entries.AsSpan())
        {
            xe.Add(entry.ToXml());
        }

        return xe;
    }

    #endregion
}