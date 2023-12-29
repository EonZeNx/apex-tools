using System.Globalization;
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
    public FRtpcV03Class[] ClassDefinitions = Array.Empty<FRtpcV03Class>();
    
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
        Container.ApplyClassDefinition(in ClassDefinitions, true);
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


    protected void SaveClassDefinitions()
    {
        var classDirectory = Settings.RtpcClassDirectory.Value;
        if (!Directory.Exists(classDirectory))
        {
            throw new DirectoryNotFoundException($"{nameof(Settings.RtpcClassDirectory)} not found: \"{classDirectory}\"");
        }

        var allContainerClasses = Container.CreateAllContainerClasses();
        ClassDefinitions = allContainerClasses
            .GroupBy(c => c.ClassHash)
            .Select(g => g.First())
            .ToArray();
        
        foreach (var containerClass in ClassDefinitions)
        {
            var name = string.Empty;
            if (Settings.PerformHashLookUp.Value)
            {
                name = HashUtils.Lookup(containerClass.ClassHash, EHashType.Class);
            }

            var fileName = $"{containerClass.ClassHash:X8}";
            if (!string.IsNullOrEmpty(name))
            {
                fileName = $"{fileName}_{name}";
            }
            
            var containerClassFile = Path.Join(classDirectory, $"{fileName}.xml");
            if (File.Exists(containerClassFile) && !Settings.RtpcUpdateClassDefinitions.Value)
            {
                continue;
            }
        
            var xd = new XDocument();
            var xe = new XElement("Definition");
            xe.SetAttributeValue(nameof(containerClass.ClassHash), $"{containerClass.ClassHash:X8}");

            if (!string.IsNullOrEmpty(name))
            {
                xe.SetAttributeValue(XElementExtensions.NameAttributeName, name);
            }
            
            foreach (var member in containerClass.Members)
            {
                var mxe = new XElement("Member");
                mxe.SetAttributeValue(nameof(member.NameHash), $"{member.NameHash:X8}");
                mxe.SetAttributeValue(nameof(member.VariantType), $"{member.VariantType.GetXmlName()}");
                mxe.SetAttributeValue(nameof(member.Name), $"{member.Name}");
                
                xe.Add(mxe);
            }
            
            xd.Add(xe);
            xe.Save(containerClassFile);
        }
    }
    
    protected void LoadClassDefinitions()
    {
        var classDirectory = Settings.RtpcClassDirectory.Value;
        if (!Directory.Exists(classDirectory))
        {
            throw new DirectoryNotFoundException($"{nameof(Settings.RtpcClassDirectory)} not found: \"{classDirectory}\"");
        }
        
        var xmlFiles = Directory.GetFiles(classDirectory, "*.xml");

        // ReSharper disable EntityNameCapturedOnly.Local
        FRtpcV03Class dummyClass;
        FRtpcV03ClassMember dummyMember;
        // ReSharper enable EntityNameCapturedOnly.Local
        
        var classDefinitions = new List<FRtpcV03Class>();
        foreach (var xmlFile in xmlFiles)
        {
            var xd = XDocument.Load(xmlFile);
            if (xd.Root is null)
            {
                throw new XmlSchemaException($"No valid root found in \"{xmlFile}\"");
            }

            var classDefinition = new FRtpcV03Class();
            
            var classHashAttribute = xd.Root?.Attribute(nameof(dummyClass.ClassHash));
            if (classHashAttribute is null)
            {
                throw new XmlSchemaException($"{nameof(dummyClass.ClassHash)} missing from \"{xmlFile}\"");
            }
            
            classDefinition.ClassHash = uint.Parse(classHashAttribute.Value, NumberStyles.HexNumber);
            
            var classNameAttribute = xd.Root?.Attribute(nameof(dummyClass.Name));
            if (classNameAttribute is not null)
            {
                classDefinition.Name = classNameAttribute.Value;
            }
            
            var memberElements = xd.Root?.Elements("Member") ?? Array.Empty<XElement>();
            foreach (var xe in memberElements)
            {
                var classMember = new FRtpcV03ClassMember();
                
                var nameHashAttribute = xe.Attribute(nameof(dummyMember.NameHash));
                if (nameHashAttribute is null)
                {
                    throw new XmlSchemaException($"{nameof(dummyMember.NameHash)} missing from \"{xmlFile}\"");
                }
                
                classMember.NameHash = uint.Parse(nameHashAttribute.Value, NumberStyles.HexNumber);
                classMember.NameHashHex = nameHashAttribute.Value;
                
                var memberNameAttribute = xe.Attribute(nameof(dummyMember.Name));
                if (memberNameAttribute is not null)
                {
                    classMember.Name = memberNameAttribute.Value;
                }
                
                var variantAttribute = xe.Attribute(nameof(dummyMember.VariantType));
                if (variantAttribute is null)
                {
                    throw new XmlSchemaException($"{nameof(dummyMember.VariantType)} missing from \"{xmlFile}\"");
                }
                
                classMember.VariantType = EVariantTypeExtensions.GetVariant(variantAttribute.Value);
                
                classDefinition.Members.Add(classMember);
            }
            
            classDefinitions.Add(classDefinition);
        }

        ClassDefinitions = classDefinitions.ToArray();
    }
}
