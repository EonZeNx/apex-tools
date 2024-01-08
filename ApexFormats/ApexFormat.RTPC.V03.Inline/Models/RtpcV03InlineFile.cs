using System.Xml;
using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Models.Data;
using ApexTools.Core.Config;
using ApexTools.Core.Interfaces;

namespace ApexFormat.RTPC.V03.Inline.Models;

/// <summary>
/// The structure for <see cref="RtpcV03InlineFile"/>
/// <br/>Version 01 - <see cref="byte"/>
/// <br/>Version 02 - <see cref="ushort"/>
/// <br/>Container count - <see cref="ushort"/>
/// <br/>Containers - <see cref="InlineContainer"/>[]
/// </summary>
public class RtpcV03InlineFile : IApexFile, IXmlFile
{
    public string ApexExtension { get; set; } = ".irtpc";
    public static string XmlName => "RTPC";
    public string XmlExtension => ".xml";
    
    public RtpcV03InlineHeader Header = new();
    protected InlineContainer[] Containers { get; set; } = Array.Empty<InlineContainer>();
    
    
    #region ApexSerializable
    
    public void FromApex(BinaryReader br)
    {
        Header = new RtpcV03InlineHeader();
        Header.FromApex(br);
        
        Containers = new InlineContainer[Header.ContainerCount];
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i] = new InlineContainer();
            Containers[i].FromApex(br);
        }

        if (Settings.PerformHashLookUp.Value)
        {
            foreach (ref var container in Containers.AsSpan())
            {
                container.LookupHash();
            }
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        Header.ToApex(bw);
        
        foreach (var container in Containers)
        {
            container.ToApex(bw);
        }
    }
    
    #endregion

    
    #region XmlSerializable

    public void FromXml(string path)
    {
        // var containers = new List<ContainerV01>();
        // var container = new ContainerV01();
        //
        // xr.ReadToDescendant(container.XmlName);
        // while (xr.NodeType == XmlNodeType.Element && xr.Name == container.XmlName)
        // {
        //     container = new ContainerV01();
        //     container.FromXml(xr);
        //     containers.Add(container);
        //         
        //     xr.ReadToNextSibling(container.XmlName);
        //     if (xr.NodeType == XmlNodeType.EndElement) xr.ReadToNextSibling(container.XmlName);
        // }
        // xr.Close();
        //
        // Containers = containers.ToArray();
        // ObjectCount = (ushort) Containers.Length;
    }

    public void ToXml(string path)
    {
        var xd = new XDocument();
        var xe = new XElement(XmlName);
        
        xe.SetAttributeValue(nameof(ApexExtension), ApexExtension);
        xe.SetAttributeValue("Inline", true);
        xe.SetAttributeValue(nameof(RtpcV03InlineHeader.Version01), RtpcV03InlineHeader.Version01);
        xe.SetAttributeValue(nameof(RtpcV03InlineHeader.Version02), RtpcV03InlineHeader.Version02);
        
        xe.ToXml(Containers);
        
        xd.Add(xe);
        
        using var xw = XmlWriter.Create(path, new XmlWriterSettings{ Indent = true, IndentChars = "\t" });
        xd.Save(xw);
    }

    #endregion
}