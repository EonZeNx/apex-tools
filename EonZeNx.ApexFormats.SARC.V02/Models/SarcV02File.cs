using System.Xml;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

namespace EonZeNx.ApexFormats.SARC.V02.Models;

/// <summary>
/// A <see cref="SarcV02File"/> file.
/// <br/> Structure:
/// <br/> Header Length - <see cref="uint"/>
/// <br/> FourCC - <see cref="EFourCc"/>
/// <br/> Version - <see cref="uint"/>
/// <br/> Data offset - <see cref="uint"/>
/// <br/> Entries array - <see cref="SarcV02Entry"/>
/// </summary>
public class SarcV02File : IApexFile, IApexSerializable, ICustomDirectorySerializable
{
    // TODO: Manager pass-through for AAf and SARC can use a manager memory stream-based binary-reader/writer.
    /* eg.
     * using var ms = new MemoryStream();
     * using var br = new BinaryReader(ms);
     *
     * Aaf.FromApex(br);
     * Sarc.FromApex(br);
     * Sarc.ToCustomDirectory(br);
     *
     * ms.ToArray();
     */
    /* eg.
     * Sarc.FromCustomDirectory(basePath);
     * 
     * using var ms = new MemoryStream();
     * using var br = new BinaryReader(ms);
     * 
     * Aaf.FromCustomFile(br);
     * Aaf.ToApex(br);
     *
     * ms.ToArray();
     */
    
    public EFourCc FourCc => EFourCc.Sarc;
    public uint Version => 0x02;
    public static string FileListName => "@files";
    
    public uint HeaderLength { get; set; }
    public uint DataOffset { get; set; }
    public SarcV02Entry[] Entries { get; set; } = Array.Empty<SarcV02Entry>();


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        HeaderLength = br.ReadUInt32();
        
        var readFourCc = br.ReadBigUInt32();
        if (readFourCc != (uint) FourCc) throw new IOException($"Character code was not valid (Expected '{(uint) FourCc}' got '{readFourCc}')");
        
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
        var xmlFileList = Path.Combine(basePath, $"{FileListName}.xml");
        if (!File.Exists(xmlFileList)) throw new FileNotFoundException($"File list '{xmlFileList}' was not found.");
        
        using var xr = XmlReader.Create(xmlFileList);
        XmlLoadReferences(xr, basePath);
    }

    public void ToCustomDirectory(string basePath)
    {
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        var xmlFileList = Path.Combine(basePath, $"{FileListName}.xml");
        
        var settings = new XmlWriterSettings{Indent = true, IndentChars = "\t"};
        using var xw = XmlWriter.Create(xmlFileList, settings);
        
        xw.WriteStartElement("ApexFile");
        XmlWriteIgnores(xw);
        XmlWriteEntries(xw, basePath);
        xw.WriteEndElement();
    }

    #endregion


    #region XmlUtils

    #region XmlWriter Utils

    private void XmlWriteEntries(XmlWriter xw, string directoryPath)
    {
        xw.WriteStartElement("Files");
            
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
        xw.WriteStartElement("Ignore");
        xw.WriteElementString("Extension", ".xml");
        xw.WriteElementString("Extension", ".bak");
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