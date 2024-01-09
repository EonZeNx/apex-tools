using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
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

        if (Settings.LookupHashes.Value)
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

    public void FromXml(XElement xe)
    {
        ApexExtension = xe.Attribute(nameof(ApexExtension))?.Value ?? ApexExtension;
        
        var containerElements = xe.Elements(InlineContainer.XmlName).ToList();
        Header.ContainerCount = (ushort) containerElements.Count;
        
        Containers = new InlineContainer[Header.ContainerCount];
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i] = containerElements[i].FromXml();
        }
    }

    public XElement ToXml()
    {
        var xe = new XElement(XmlName);
        
        xe.SetAttributeValue(nameof(ApexExtension), ApexExtension);
        xe.SetAttributeValue("Inline", true);
        xe.SetAttributeValue(nameof(RtpcV03InlineHeader.Version01), RtpcV03InlineHeader.Version01);
        xe.SetAttributeValue(nameof(RtpcV03InlineHeader.Version02), RtpcV03InlineHeader.Version02);
        
        xe.ToXml(Containers);

        return xe;
    }

    #endregion
}