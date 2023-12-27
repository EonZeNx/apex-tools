using System.Runtime.InteropServices;
using System.Xml.Linq;
using ApexTools.JC4.RTPC.V03.NewModels.Utils;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03Container
{
    public RtpcV03ContainerHeader Header = new();
    public RtpcV03PropertyHeader[] PropertyHeaders = Array.Empty<RtpcV03PropertyHeader>();
    public RtpcV03ContainerHeader[] ContainerHeaders = Array.Empty<RtpcV03ContainerHeader>();
    public uint ValidProperties = 0;

    public RtpcV03Container[] Containers = Array.Empty<RtpcV03Container>();

    public static string XmlName = "Container";

    public RtpcV03Container() {}
}

public static class RtpcV03ContainerExtension
{
    public static void LookupNameHash(this ref RtpcV03Container container)
    {
        container.Header.LookupNameHash();
        
        for (var i = 0; i < container.PropertyHeaders.Length; i++)
        {
            container.PropertyHeaders[i].LookupNameHash();
        }
        
        for (var i = 0; i < container.Containers.Length; i++)
        {
            container.Containers[i].LookupNameHash();
        }
    }
    
    public static void Sort(this ref RtpcV03Container container)
    {
        container.PropertyHeaders = container.PropertyHeaders
            .OrderBy(ph => string.IsNullOrEmpty(ph.Name))
            .ThenBy(ph => ph.Name)
            .ThenBy(ph => ph.NameHash)
            .ToArray();

        for (var i = 0; i < container.Containers.Length; i++)
        {
            container.Containers[i].Sort();
        }
    }
    
    public static uint DataSize(this RtpcV03Container container, bool withValid = false)
    {
        var propertySize = container.Header.PropertyCount * RtpcV03PropertyHeader.SizeOf();
        var containerHeaderSize = container.Header.ContainerCount * RtpcV03ContainerHeader.SizeOf(true);
        const int validPropertySize = 4;
        
        var result = (uint) (propertySize + containerHeaderSize +
                             container.Containers.Sum(c => c.DataSize()));
        if (withValid)
        {
            result += validPropertySize;
        }
        
        return result;
    }
    
    public static uint SetBodyOffset(this RtpcV03Container container, uint containerOffset)
    {
        for (var i = 0; i < container.Containers.Length; i++)
        {
            container.ContainerHeaders[i].BodyOffset = containerOffset;
            container.Containers[i].Header.BodyOffset = containerOffset;
            containerOffset += container.Containers[i].DataSize(true);
        }
        
        for (var j = 0; j < container.Containers.Length; j++)
        {
            containerOffset = container.Containers[j].SetBodyOffset(containerOffset);
        }

        return containerOffset;
    }
    
    public static IEnumerable<RtpcV03PropertyHeader> GetAllPropertyHeaders(this RtpcV03Container container)
    {
        var result = new List<RtpcV03PropertyHeader>(container.PropertyHeaders);
        foreach (var subContainer in container.Containers)
        {
            result.AddRange(subContainer.GetAllPropertyHeaders());
        }

        return result;
    }
    
    public static int CountAllPropertyHeaders(this RtpcV03Container container)
    {
        return container.Header.PropertyCount + 
               container.Containers.Sum(subContainer => subContainer.CountAllPropertyHeaders());
    }
    
    public static int CountAllContainerHeaders(this RtpcV03Container container)
    {
        return container.Header.ContainerCount + 
               container.Containers.Sum(subContainer => subContainer.CountAllContainerHeaders());
    }
    
    // Container header and body are separate
    public static RtpcV03Container ReadRtpcV03Container(this BinaryReader br, RtpcV03ContainerHeader header)
    {
        var result = new RtpcV03Container
        {
            Header = header,
            PropertyHeaders = new RtpcV03PropertyHeader[header.PropertyCount],
            ContainerHeaders = new RtpcV03ContainerHeader[header.ContainerCount],
            Containers = new RtpcV03Container[header.ContainerCount]
        };

        for (var i = 0; i < header.PropertyCount; i++)
        {
            result.PropertyHeaders[i] = br.ReadRtpcV03PropertyHeader();
        }

        for (var i = 0; i < header.ContainerCount; i++)
        {
            result.ContainerHeaders[i] = br.ReadRtpcV03ContainerHeader();
        }

        result.ValidProperties = br.ReadUInt32();

        for (var i = 0; i < header.ContainerCount; i++)
        {
            result.Containers[i] = br.ReadRtpcV03Container(result.ContainerHeaders[i]);
        }

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03Container container, in RtpcV03ValueOffsetMaps voMaps)
    {
        foreach (var propertyHeader in container.PropertyHeaders)
        {
            bw.Write(propertyHeader, voMaps);
        }

        foreach (var containerHeader in container.ContainerHeaders)
        {
            bw.Write(containerHeader);
        }
        
        bw.Write(container.ValidProperties);

        foreach (var subContainer in container.Containers)
        {
            bw.Write(subContainer, voMaps);
        }
    }
    
    public static void Write(this XElement pxe, RtpcV03Container container, in RtpcV03OffsetValueMaps ovMaps)
    {
        var xe = new XElement(RtpcV03Container.XmlName);
        
        xe.WriteNameOrHash(container.Header.NameHash, container.Header.Name);
        
        foreach (var propertyHeader in container.PropertyHeaders)
        {
            xe.Write(propertyHeader, ovMaps);
        }
        
        foreach (var subContainer in container.Containers)
        {
            xe.Write(subContainer, ovMaps);
        }
        
        pxe.Add(xe);
    }
    
    public static RtpcV03Container ReadRtpcV03Container(this XElement xe)
    {
        var properties = xe.Elements()
            .Where(e => e.Name.ToString() != RtpcV03Container.XmlName)
            .ToArray();
        var containers = xe.Elements(RtpcV03Container.XmlName)
            .ToArray();

        var header = new RtpcV03ContainerHeader
        {
            NameHash = xe.GetNameHash(),
            ContainerCount = (ushort) containers.Length,
            PropertyCount = (ushort) properties.Length
        };
        var result = new RtpcV03Container
        {
            Header = header,
            PropertyHeaders = new RtpcV03PropertyHeader[header.PropertyCount],
            ContainerHeaders = new RtpcV03ContainerHeader[header.ContainerCount],
            Containers = new RtpcV03Container[header.ContainerCount]
        };

        for (var i = 0; i < header.PropertyCount; i++)
        {
            var node = properties?[i];
            if (node != null)
            {
                result.PropertyHeaders[i] = node.ReadRtpcV03PropertyHeader();
            }
        }

        for (var i = 0; i < header.ContainerCount; i++)
        {
            var node = containers?[i];
            if (node != null)
            {
                result.Containers[i] = node.ReadRtpcV03Container();
                result.ContainerHeaders[i] = result.Containers[i].Header;
            }
        }

        result.ValidProperties = (uint) (properties?.Count(p => p.Name != EVariantType.Unassigned.GetXmlName()) ?? 0);

        return result;
    }
}