using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Utils;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct RtpcV03Container
{
    public RtpcV03ContainerHeader Header;
    public RtpcV03PropertyHeader[] PropertyHeaders;
    public RtpcV03ContainerHeader[] ContainerHeaders;
    public uint ValidProperties;

    public bool Flat;
    public RtpcV03Container[] Containers;

    public static string XmlName = "Container";

    public RtpcV03Container()
    {
        Header = new RtpcV03ContainerHeader();
        PropertyHeaders = Array.Empty<RtpcV03PropertyHeader>();
        ContainerHeaders = Array.Empty<RtpcV03ContainerHeader>();
        ValidProperties = 0;
        
        Flat = false;
        Containers = Array.Empty<RtpcV03Container>();
    }
}

public static class RtpcV03ContainerExtension
{
    public static void SetContainerNameHash(ref this RtpcV03Container container, ref int id)
    {
        foreach (ref var subContainer in container.Containers.AsSpan())
        {
            subContainer.Header.NameHash = (uint) id;
            id += 1;
        }
        
        foreach (ref var subContainer in container.Containers.AsSpan())
        {
            subContainer.SetContainerNameHash(ref id);
        }

        container.ContainerHeaders = container.Containers.Select(c => c.Header).ToArray();
    }
    
    public static void ApplyClassDefinition(ref this RtpcV03Container container, in Dictionary<uint, List<FRtpcV03ClassDefinition>> classDefinitions, bool isRoot = false)
    {
        if (!isRoot)
        {
            var classHash = container.GetClassHash();
            var classHashHex = $"{classHash:X8}";
            if (!classDefinitions.ContainsKey(classHash)) throw new Exception("Unknown class hash");

            var properties = container.PropertyHeaders;
            var propertiesNoUnassigned = properties
                .Where(h => h.VariantType != EVariantType.Unassigned)
                .ToArray();

            var definitions = classDefinitions[classHash].Where(d => d.Members.Count == properties.Length);
            var definition = new FRtpcV03ClassDefinition();
            foreach (var classDefinition in definitions)
            {
                var filtered = classDefinition.Members
                    .Where(m => m.VariantType != EVariantType.Unassigned)
                    .Select(h => h.NameHash);

                var temp = filtered.Except(propertiesNoUnassigned.Select(h => h.NameHash));
                if (temp.Any())
                {
                    continue;
                }

                definition = classDefinition;
                break;
            }
            
            if (definition.ClassHash == 0) throw new Exception("Unknown class definition");
            
            var orderedProperties = new List<RtpcV03PropertyHeader>();
            foreach (var classMember in definition.Members)
            {
                var property = new RtpcV03PropertyHeader();
                if (classMember.VariantType != EVariantType.Unassigned)
                {
                    property = properties.First(h => h.NameHash == classMember.NameHash);
                }
                
                orderedProperties.Add(property);
            }

            // TODO: Compare sorted & unsorted properties
            container.PropertyHeaders = orderedProperties.ToArray();
            container.Header.PropertyCount = (ushort) container.PropertyHeaders.Length;
        }

        foreach (ref var subContainer in container.Containers.AsSpan())
        {
            subContainer.ApplyClassDefinition(in classDefinitions);
        }
    }

    public static void UnFlatten(ref this RtpcV03Container container, in IList<uint> parentIndexArray)
    {
        foreach (ref var subContainer in container.Containers.AsSpan())
        {
            subContainer.Flat = true;
        }
        
        var i = parentIndexArray.Count - 1;
        while (i >= 0)
        {
            var parentIndex = parentIndexArray[i];
            if (parentIndex != 0xFFFFFFFF)
            {
                var subContainer = container.Containers[i];
                subContainer.Flat = true;

                container.Containers[parentIndex].Containers = container.Containers[parentIndex].Containers.Append(subContainer).ToArray();
            }

            i -= 1;
        }

        // Mask parent index array with target index
        var indicesToRemove = parentIndexArray
            .Select((pIndex, j) => (uint) Math.Max(pIndex, j))
            .Where(j => j < uint.MaxValue);
        
        container.Containers = container.Containers
            .Where((_, j) => !indicesToRemove.Contains((uint) j))
            .ToArray();
        container.ContainerHeaders = container.ContainerHeaders
            .Where((_, j) => !indicesToRemove.Contains((uint) j))
            .ToArray();
    }

    public static IEnumerable<RtpcV03Container> Flatten(ref this RtpcV03Container container, uint parentIndex, ref List<uint> parentIndices)
    {
        var flatContainers = new List<RtpcV03Container>();
        
        parentIndices.Add(parentIndex);
        var index = (uint) parentIndices.Count - 1;
        
        foreach (ref var subContainer in container.Containers.AsSpan())
        {
            if (!subContainer.Flat) continue;
            flatContainers.AddRange(subContainer.Flatten(index, ref parentIndices));
        }
        
        container.Containers = container.Containers.Where(c => !c.Flat).ToArray();
        container.ContainerHeaders = container.Containers.Select(c => c.Header).ToArray();
        container.Header.ContainerCount = (ushort) container.Containers.Length;
        
        flatContainers.Insert(0, container);

        return flatContainers;
    }

    public static void CreateRootFlattenedProperties(ref this RtpcV03Container container, ref RtpcV03ValueOffsetMaps voMaps, in List<uint> parentIndices)
    {
        var properties = new List<RtpcV03PropertyHeader>();
        
        { // Indices
            var indicesProperty = new RtpcV03PropertyHeader
            {
                NameHash = 0xCFD7B43E,
                RawData = new byte[4],
                VariantType = EVariantType.UInteger32Array,
                XmlData = string.Join(",", parentIndices)
            };
            properties.Add(indicesProperty);
            
            voMaps.U32ArrayOffsetMap.Add(parentIndices, 0);
        }

        { // Class hash
            var classHashArray = container.Containers.Select(c => c.GetClassHash()).ToArray();
            var classHashArrayProperty = new RtpcV03PropertyHeader
            {
                NameHash = 0x6AE2DDA0,
                RawData = new byte[4],
                VariantType = EVariantType.UInteger32Array,
                XmlData = string.Join(",", classHashArray)
            };
            properties.Add(classHashArrayProperty);
            
            voMaps.U32ArrayOffsetMap.Add(classHashArray, 0);
        }

        { // Object ID
            var oIdArray = container.Containers.Select(c => c.GetObjectId()).ToArray();
            var byteArray = oIdArray.SelectMany(BitConverter.GetBytes).ToArray();
            var hexByteArray = byteArray.Select(b => $"{b:X2}");
            
            var classHashArrayProperty = new RtpcV03PropertyHeader
            {
                NameHash = 0x0584FFCF,
                RawData = new byte[4],
                VariantType = EVariantType.ByteArray,
                XmlData = string.Join(",", hexByteArray)
            };
            properties.Add(classHashArrayProperty);
            
            voMaps.ByteArrayOffsetMap.Add(byteArray, 0);
        }

        container.PropertyHeaders = container.PropertyHeaders.Concat(properties).ToArray();
        container.Header.PropertyCount = (ushort) container.PropertyHeaders.Length;

        container.Header.NameHash = ByteUtils.ReverseBytes(0x2A527DAA);
    }
    
    public static void LookupNameHash(ref this RtpcV03Container container)
    {
        for (var i = 0; i < container.PropertyHeaders.Length; i++)
        {
            container.PropertyHeaders[i].LookupNameHash();
        }
        
        for (var i = 0; i < container.Containers.Length; i++)
        {
            container.Containers[i].LookupNameHash();
        }
    }
    
    public static void Sort(ref this RtpcV03Container container)
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
    
    public static uint DataSize(in this RtpcV03Container container, bool withValid = false)
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
    
    public static uint SetBodyOffset(ref this RtpcV03Container container, uint containerOffset)
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
    
    public static IEnumerable<RtpcV03PropertyHeader> GetAllPropertyHeaders(in this RtpcV03Container container)
    {
        var result = new List<RtpcV03PropertyHeader>(container.PropertyHeaders);
        foreach (var subContainer in container.Containers)
        {
            result.AddRange(subContainer.GetAllPropertyHeaders());
        }

        return result;
    }
    
    public static IEnumerable<FRtpcV03ClassDefinition> CreateAllClassDefinitions(in this RtpcV03Container container)
    {
        var result = new List<FRtpcV03ClassDefinition>();
        result.AddRange(container.Containers.Select(sc => sc.CreateContainerClass()));

        foreach (var subContainer in container.Containers)
        {
            result.AddRange(subContainer.CreateAllClassDefinitions());
        }

        return result;
    }

    public static ulong GetObjectId(in this RtpcV03Container container)
    {
        var header = container.PropertyHeaders.First(h => h.NameHash == ByteUtils.ReverseBytes(0x0584FFCF));
        
        return ulong.Parse(header.XmlData, NumberStyles.HexNumber);
    }
    
    public static uint GetClassHash(in this RtpcV03Container container)
    {
        var classHashHeader = container.PropertyHeaders.First(h => h.NameHash == ByteUtils.ReverseBytes(0xE65940D0));
        
        return BitConverter.ToUInt32(classHashHeader.RawData);
    }
    
    public static FRtpcV03ClassDefinition CreateContainerClass(in this RtpcV03Container container)
    {
        var result = new FRtpcV03ClassDefinition
        {
            ClassHash = container.GetClassHash(),
            Members = container.PropertyHeaders.Select(h => new FRtpcV03ClassDefinitionMember
            {
                NameHash = h.NameHash,
                VariantType = h.VariantType,
                Name = Settings.PerformHashLookUp.Value ? HashUtils.Lookup(h.NameHash) : string.Empty
            }).ToList()
        };

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
    
    public static void Write(this BinaryWriter bw, in RtpcV03Container container, in RtpcV03ValueOffsetMaps voMaps)
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
    
    public static void Write(this XElement pxe, in RtpcV03Container container, in RtpcV03OffsetValueMaps ovMaps, bool isRoot = false)
    {
        var xe = new XElement(RtpcV03Container.XmlName);

        if (!container.Flat)
        {
            xe.SetAttributeValue(nameof(container.Flat), container.Flat);
        }
        
        foreach (var header in container.PropertyHeaders)
        {
            xe.Write(header, ovMaps);
        }
        
        foreach (var subContainer in container.Containers)
        {
            xe.Write(subContainer, ovMaps);
        }
        
        pxe.Add(xe);
    }
    
    public static RtpcV03Container ReadRtpcV03Container(this XElement xe, bool isRoot = false)
    {
        var properties = xe.Elements()
            .Where(e => e.Name.ToString() != RtpcV03Container.XmlName)
            .ToList();
        var containers = xe.Elements(RtpcV03Container.XmlName)
            .ToList();
        
        var header = new RtpcV03ContainerHeader
        {
            ContainerCount = (ushort) containers.Count,
            PropertyCount = (ushort) properties.Count
        };
        
        var result = new RtpcV03Container
        {
            Header = header,
            PropertyHeaders = new RtpcV03PropertyHeader[header.PropertyCount],
            ContainerHeaders = new RtpcV03ContainerHeader[header.ContainerCount],
            Containers = new RtpcV03Container[header.ContainerCount]
        };
        
        { // Flat (optional)
            var flatAttribute = xe.Attribute($"{nameof(result.Flat)}");
            
            result.Flat = true;
            if (flatAttribute is not null)
            {
                result.Flat = bool.Parse(flatAttribute.Value);
            }
        }

        for (var i = 0; i < header.PropertyCount; i++)
        {
            var node = properties?[i];
            if (node == null) continue;
            
            result.PropertyHeaders[i] = node.ReadRtpcV03PropertyHeader();
        }

        for (var i = 0; i < header.ContainerCount; i++)
        {
            var node = containers?[i];
            if (node == null) continue;
            
            result.Containers[i] = node.ReadRtpcV03Container();
            result.ContainerHeaders[i] = result.Containers[i].Header;
        }

        result.ValidProperties = (uint) (properties?.Count(p => p.Name != EVariantType.Unassigned.GetXmlName()) ?? 0);

        return result;
    }
}