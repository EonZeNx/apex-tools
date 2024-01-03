using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Abstractions;
using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Flat.Models;

public class RtpcV03File : IApexFile, IXmlFile
{
    public RtpcV03Header Header = new();
    public RtpcV03Container Container = new();
    
    public RtpcV03OffsetValueMaps OvMaps = new();
    public RtpcV03ValueOffsetMaps VoMaps = new();
    public Dictionary<uint, List<FRtpcV03ClassDefinition>> ClassDefinitions = new();
    
    public string ApexExtension { get; set; } = ".rtpc";
    public static string XmlName => "RTPC";
    public string XmlExtension => ".xml";

    public void FromApex(BinaryReader br)
    {
        br.Seek(0);
        
        Header = br.ReadRtpcV03Header();
        var containerHeader = br.ReadRtpcV03ContainerHeader();
        Container = br.ReadRtpcV03Container(containerHeader);
        Container.Flat = true; // To distinguish between RTPC v3 and flat RTPC v3, root container must have flat = true
        
        var allPropertyHeaders = Container.GetAllPropertyHeaders();
        var uniqueOffsets = allPropertyHeaders
            .Where(ph => ph.VariantType is not
                (EVariantType.Unassigned or EVariantType.UInteger32 or EVariantType.Float32))
            .GroupBy(ph => BitConverter.ToUInt32(ph.RawData))
            .Select(g => g.First())
            .ToArray();
        
        OvMaps.Create(br, uniqueOffsets);

        SaveClassDefinitions();
    }

    public void ToApex(BinaryWriter bw)
    {
        Container.ApplyClassDefinition(in ClassDefinitions, true);
        
        // var parentIndices = new List<uint>();
        // var flatContainers = new List<RtpcV03Container>();
        //
        // foreach (ref var subContainer in Container.Containers.AsSpan())
        // {
        //     if (!subContainer.Flat) continue;
        //     flatContainers.AddRange(subContainer.Flatten(0xFFFFFFFF, ref parentIndices));
        // }
        
        // Container.Containers = flatContainers.ToArray();
        // Container.ContainerHeaders = flatContainers.Select(c => c.Header).ToArray();
        Container.Header.ContainerCount = (ushort) Container.Containers.Length;
        Container.Header.NameHash = ByteUtils.ReverseBytes(0x2A527DAA);
        
        // Container.CreateRootFlattenedProperties(ref VoMaps, in parentIndices);

        var containerId = 1;
        Container.SetContainerNameHash(ref containerId);
        
        var containerOffset = (uint) (RtpcV03Header.SizeOf() + 1 * RtpcV03ContainerHeader.SizeOf());
        Container.Header.BodyOffset = containerOffset;
        {
            var propertySize = Container.Header.PropertyCount * RtpcV03PropertyHeader.SizeOf();
            var containerHeaderSize = Container.Header.ContainerCount * RtpcV03ContainerHeader.SizeOf();
            
            // +4 from root container valid property count
            containerOffset += (uint) (propertySize + containerHeaderSize + 4);
        }
        Container.SetBodyOffset(containerOffset);
        
        var propertyCount = Container.CountAllPropertyHeaders();
        var containerCount = Container.CountAllContainerHeaders();
        
        var propertyDataOffset = (uint) (RtpcV03Header.SizeOf() + 
            RtpcV03ContainerHeader.SizeOf(true) +
            propertyCount * RtpcV03PropertyHeader.SizeOf() + 
            containerCount * RtpcV03ContainerHeader.SizeOf(true)
        );
        
        bw.Seek((int) propertyDataOffset, SeekOrigin.Begin);
        bw.Write(VoMaps);
        bw.Seek(0, SeekOrigin.Begin);
        
        bw.Write(Header);
        bw.Write(Container.Header);
        bw.Write(Container, VoMaps);
    }

    public void FromXml(string targetPath)
    {
        LoadClassDefinitions();
        
        var xd = XDocument.Load(targetPath);
        VoMaps.Create(xd);
        
        ApexExtension = xd.Root?.Attribute(nameof(ApexExtension))?.Value ?? ApexExtension;
        Header = new RtpcV03Header
        {
            FourCc = EFourCc.Rtpc,
            Version = 3
        };
        
        var rtpcNode = xd.Element(XmlName)?.Element(RtpcV03Container.XmlName);
        if (rtpcNode is null)
        {
            throw new XmlSchemaException($"{XmlName} missing from \"{targetPath}\"");
        }
        
        Container = rtpcNode.ReadRtpcV03Container(true);
    }

    public void ToXml(string targetPath)
    {
        if (Settings.PerformHashLookUp.Value)
        {
            Container.LookupNameHash();
        }
        
        if (Settings.RtpcSortProperties.Value)
        {
            Container.Sort();
        }
        
        var xd = new XDocument();
        var xe = new XElement(XmlName);
        
        xe.SetAttributeValue(nameof(ApexExtension), ApexExtension);
        xe.SetAttributeValue(nameof(Header.Version), Header.Version);
        xe.SetAttributeValue(nameof(Container.Flat), true);

        xe.Write(Container, OvMaps, true);
        
        xd.Add(xe);
        xd.Save(targetPath);
    }


    /// <summary>
    /// Class definitions can have optional properties, track each one
    /// </summary>
    /// <exception cref="DirectoryNotFoundException"></exception>
    protected void SaveClassDefinitions()
    {
        var definitionDirectory = Settings.RtpcClassDefinitionDirectory.Value;
        if (!Directory.Exists(definitionDirectory))
        {
            throw new DirectoryNotFoundException($"{nameof(Settings.RtpcClassDefinitionDirectory)} not found: \"{definitionDirectory}\"");
        }

        var unique = Container.CreateAllClassDefinitions()
            .Distinct()
            .ToArray();
        
        if (Settings.PerformHashLookUp.Value)
        {
            var uniqueSpan = unique.AsSpan();
            foreach (ref var definition in uniqueSpan)
            {
                definition.Name = HashUtils.Lookup(definition.ClassHash, EHashType.Class);
            }
        }

        ClassDefinitions = unique
            .GroupBy(d => d.ClassHash)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (classHash, definitions) in ClassDefinitions)
        {
            var fileName = $"{classHash:X8}";
            var definitionFile = Directory.GetFiles(definitionDirectory, $"{fileName}*.xml").FirstOrDefault();
            if (string.IsNullOrEmpty(definitionFile))
            {
                var className = definitions.First().Name;
                if (!string.IsNullOrEmpty(className))
                {
                    fileName += $"_{className}";
                }
                
                definitionFile = Path.Join(definitionDirectory, $"{fileName}.xml");
            }
            
            var xd = File.Exists(definitionFile) ? XDocument.Load(definitionFile) : new XDocument();
            if (xd.Root is null)
            {
                xd.Add(new XElement("Definitions"));
            }

            var savedDXeList = xd.Descendants(FRtpcV03ClassDefinition.XmlName)
                .Select(xe => xe.DefinitionFromXElement())
                .ToArray();
            var dxeList = definitions.Where(d => !savedDXeList.Contains(d))
                .Select(d => d.CreateXElement())
                .ToArray();

            foreach (var dxe in dxeList)
            {
                xd.Root?.Add(dxe);
            }
            
            xd.Save(definitionFile);
        }
    }
    
    protected void LoadClassDefinitions()
    {
        var classDirectory = Settings.RtpcClassDefinitionDirectory.Value;
        if (!Directory.Exists(classDirectory))
        {
            throw new DirectoryNotFoundException($"{nameof(Settings.RtpcClassDefinitionDirectory)} not found: \"{classDirectory}\"");
        }
        
        var xmlFiles = Directory.GetFiles(classDirectory, "*.xml");
        var classDefinitions = new Dictionary<uint, List<FRtpcV03ClassDefinition>>();
        
        foreach (var xmlFile in xmlFiles)
        {
            var xd = XDocument.Load(xmlFile);
            if (xd.Root is null)
            {
                throw new XmlSchemaException($"No valid root found in \"{xmlFile}\"");
            }

            var definitions = xd.DefinitionsFromXDocument();
            if (definitions.Count == 0) continue;
            
            var first = definitions.First();
            classDefinitions.Add(first.ClassHash, definitions);
        }

        ClassDefinitions = classDefinitions;
    }
}
