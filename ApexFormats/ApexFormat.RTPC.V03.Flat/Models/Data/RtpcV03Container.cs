using System.Xml.Linq;
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
    public static List<uint> ClassHashes = new()
    {
        // 49639034,
        // 130186728,
        // 131178563,
        // 223103978,
        // 302102125,
        // 349766449,
        // 784662036,
        // 854750954,
        // 985770513,
        // 1078578751,
        // 1089081654,
        // 1256626520,
        // 1263718194,
        // 1267387985,
        // 1310083454,
        // 1313213895,
        // 1313580645,
        // 1516962764,
        // 1538251111,
        // 1603476123,
        // TODO: This is fucked 1713244665,
        // 1732484761,
        // 1822804832,
        // 1891047768,
        // 2029116022,
        // 2071185270,
        // 2350009519,
        // 2400941364,
        // 2495072499,
        // 2510826771,
        // 3166021335,
        // 3221305917,
        // 3283921524,
        // 3359915272,
        // 3431567010,
        // 3431661127,
        // TODO: This is fucked 3457537195,
        // 3478510451,
        // 3511785762,
        // 3548386982,
        // 3608552463,
        // 3665801671,
        // 3672051056,
        // 3734839025,
        // 3788215443,
        // 4029389064,
        // 4095556453,
        // 4231742465
    };

    public static void UnFlatten(this ref RtpcV03Container container, IList<uint> parentIndexArray)
    {
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
            .Where((c, j) => !indicesToRemove.Contains((uint) j))
            .ToArray();
        container.ContainerHeaders = container.ContainerHeaders
            .Where((c, j) => !indicesToRemove.Contains((uint) j))
            .ToArray();
    }
    
    public static void LookupNameHash(this ref RtpcV03Container container)
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
    
    public static IEnumerable<FRtpcV03Class> CreateAllContainerClasses(this RtpcV03Container container)
    {
        var result = new List<FRtpcV03Class>();
        result.AddRange(container.Containers.Select(sc => sc.CreateContainerClass()));

        return result;
    }
    
    public static FRtpcV03Class CreateContainerClass(this RtpcV03Container container)
    {
        var classHashHeader = container.PropertyHeaders.First(h => h.NameHash == ByteUtils.ReverseBytes(0xE65940D0));
        var result = new FRtpcV03Class
        {
            ClassHash = BitConverter.ToUInt32(classHashHeader.RawData),
            Members = container.PropertyHeaders.Select(h => new FRtpcV03ClassMember
            {
                NameHash = h.NameHash,
                VariantType = h.VariantType
            }).ToList()
        }.FilterDefaultMembers();

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
    
    public static void Write(this XElement pxe, in RtpcV03Container container, in RtpcV03OffsetValueMaps ovMaps, in FRtpcV03Class[] classDefinitions, bool isRoot = false)
    {
        var xe = new XElement(RtpcV03Container.XmlName);
        
        xe.SetAttributeValue(nameof(container.Flat), container.Flat);

        if (isRoot)
        {
            var unknown = container.PropertyHeaders.First(h => h.NameHash == 0x95C1191D);
            xe.SetAttributeValue("Unknown", BitConverter.ToUInt32(unknown.RawData));
        }
        else
        {
            {
                // Class
                var classHash = ByteUtils.ReverseBytes(0xE65940D0);
                var classHeader = container.PropertyHeaders.First(h => h.NameHash == classHash);

                var classValue = BitConverter.ToUInt32(classHeader.RawData);
                var classStr = $"{classValue:X8}";
                
                if (Settings.PerformHashLookUp.Value)
                {
                    var result = HashUtils.Lookup(classValue, EHashType.Class);
                    classStr = string.IsNullOrEmpty(result) ? classStr : result;
                }
                
                xe.SetAttributeValue("Class", classStr);
            }

            {
                // Object ID
                var objectIdHash = ByteUtils.ReverseBytes(0x0584FFCF);
                var oIdHeader = container.PropertyHeaders.First(h => h.NameHash == objectIdHash);
                
                var oIdOffset = BitConverter.ToUInt32(oIdHeader.RawData);
                var oId = ovMaps.OffsetObjectIdMap[oIdOffset];
                xe.SetAttributeValue("OID", $"{oId:X16}");
            }
            
            {
                // Name (optional)
                var nameHash = ByteUtils.ReverseBytes(0x84B61AD3);
                if (container.PropertyHeaders.Any(h => h.NameHash == nameHash))
                {
                    var nameHeader = container.PropertyHeaders.First(h => h.NameHash == nameHash);
                    
                    var nameOffset = BitConverter.ToUInt32(nameHeader.RawData);
                    var name = ovMaps.OffsetStringMap[nameOffset];
                    xe.SetAttributeValue("Name", name);
                }
            }

            var writableProperties = FRtpcV03ClassExtensions.FilterDefaultMembers(container.PropertyHeaders)
                .Where(h => h.VariantType != EVariantType.Unassigned);
            foreach (var header in writableProperties)
            {
                xe.Write(header, ovMaps);
            }
        }
        
        foreach (var subContainer in container.Containers)
        {
            xe.Write(subContainer, ovMaps, classDefinitions);
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