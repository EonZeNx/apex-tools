﻿using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Abstractions;
using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Flat.Utils;
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
        var parentIndices = new List<uint>();
        var flatContainers = new List<RtpcV03Container>();
        
        foreach (ref var subContainer in Container.Containers.AsSpan())
        {
            if (!subContainer.Flat) continue;
            flatContainers.AddRange(subContainer.Flatten(0xFFFFFFFF, ref parentIndices));
        }
        
        Container.Containers = flatContainers.ToArray();
        Container.ContainerHeaders = flatContainers.Select(c => c.Header).ToArray();
        Container.Header.ContainerCount = (ushort) Container.Containers.Length;
        
        Container.CreateRootFlattenedProperties(ref VoMaps, in parentIndices);

        var containerId = 1;
        Container.SetContainerNameHash(ref containerId);
        
        var containerOffset = (uint) (RtpcV03Header.SizeOf() + 1 * RtpcV03ContainerHeader.SizeOf());
        Container.Header.BodyOffset = containerOffset;
        {
            var propertySize = Container.Header.PropertyCount * RtpcV03PropertyHeader.SizeOf();
            var containerHeaderSize = Container.Header.ContainerCount * RtpcV03ContainerHeader.SizeOf(true);
            
            containerOffset += (uint) (propertySize + containerHeaderSize);
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
        // TODO: Readd this
        // Container.ApplyClassDefinition(in ClassDefinitions, true);
    }

    public void ToXml(string targetPath)
    {
        var parentIndexArrayOffset = Container.PropertyHeaders
            .First(h => h.NameHash == 0xCFD7B43E).RawData;
        
        var parentIndexArray = OvMaps.OffsetU32ArrayMap[BitConverter.ToUInt32(parentIndexArrayOffset)];
        Container.UnFlatten(in parentIndexArray);
        
        // TODO: Support 0x8F1D6E5A byte array ADF
        // Other 3 properties can be inferred by container structure
        Container.PropertyHeaders = Container.PropertyHeaders
            .Where(h => h.NameHash == 0x95C1191D)
            .ToArray();
        
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
        xe.SetAttributeValue("Flat", true);

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
                continue;
            }
            
            var xd = File.Exists(definitionFile) ? XDocument.Load(definitionFile) : new XDocument();
            if (xd.Root is null)
            {
                xd.Add(new XElement("Definitions"));
            }

            var savedDXeList = xd.Descendants("Definition")
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

        // ReSharper disable EntityNameCapturedOnly.Local
        FRtpcV03ClassDefinition dummyClassDefinition;
        FRtpcV03ClassDefinitionMember dummyDefinitionMember;
        // ReSharper enable EntityNameCapturedOnly.Local
        
        var classDefinitions = new List<FRtpcV03ClassDefinition>();
        foreach (var xmlFile in xmlFiles)
        {
            var xd = XDocument.Load(xmlFile);
            if (xd.Root is null)
            {
                throw new XmlSchemaException($"No valid root found in \"{xmlFile}\"");
            }

            var classDefinition = new FRtpcV03ClassDefinition();
            
            var classHashAttribute = xd.Root?.Attribute(nameof(dummyClassDefinition.ClassHash));
            if (classHashAttribute is null)
            {
                throw new XmlSchemaException($"{nameof(dummyClassDefinition.ClassHash)} missing from \"{xmlFile}\"");
            }
            
            classDefinition.ClassHash = uint.Parse(classHashAttribute.Value, NumberStyles.HexNumber);
            
            var classNameAttribute = xd.Root?.Attribute(nameof(dummyClassDefinition.Name));
            if (classNameAttribute is not null)
            {
                classDefinition.Name = classNameAttribute.Value;
            }
            
            var memberElements = xd.Root?.Elements("Member") ?? Array.Empty<XElement>();
            foreach (var xe in memberElements)
            {
                var classMember = new FRtpcV03ClassDefinitionMember();
                
                var nameHashAttribute = xe.Attribute(nameof(dummyDefinitionMember.NameHash));
                if (nameHashAttribute is null)
                {
                    throw new XmlSchemaException($"{nameof(dummyDefinitionMember.NameHash)} missing from \"{xmlFile}\"");
                }
                
                classMember.NameHash = uint.Parse(nameHashAttribute.Value, NumberStyles.HexNumber);
                classMember.NameHashHex = nameHashAttribute.Value;
                
                var memberNameAttribute = xe.Attribute(nameof(dummyDefinitionMember.Name));
                if (memberNameAttribute is not null)
                {
                    classMember.Name = memberNameAttribute.Value;
                }
                
                var variantAttribute = xe.Attribute(nameof(dummyDefinitionMember.VariantType));
                if (variantAttribute is null)
                {
                    throw new XmlSchemaException($"{nameof(dummyDefinitionMember.VariantType)} missing from \"{xmlFile}\"");
                }
                
                classMember.VariantType = EVariantTypeExtensions.GetVariant(variantAttribute.Value);
                
                classDefinition.Members.Add(classMember);
            }
            
            classDefinitions.Add(classDefinition);
        }

        // TODO: Readd this
        // ClassDefinitions = classDefinitions.ToArray();
    }
}
