using System.Xml.Linq;
using ApexFormat.RTPC.V03.Flat.Abstractions;
using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;

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
        var containerOffset = (uint) (RtpcV03Header.SizeOf() + 1 * RtpcV03ContainerHeader.SizeOf());
        Container.Header.BodyOffset = containerOffset;
        {
            var propertySize = Container.Header.PropertyCount * RtpcV03PropertyHeader.SizeOf();
            var containerHeaderSize = Container.Header.ContainerCount * RtpcV03ContainerHeader.SizeOf();
            const int validPropertySize = 4;
            
            containerOffset += (uint) (propertySize + containerHeaderSize + validPropertySize);
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
        var xd = XDocument.Load(targetPath);
        VoMaps.Create(xd);

        ApexExtension = xd.Root?.Attribute(nameof(ApexExtension))?.Value ?? ApexExtension;
        Header = new RtpcV03Header
        {
            FourCc = EFourCc.Rtpc,
            Version = 3
        };
        
        var rtpcNode = xd.Element(XmlName)?.Element(RtpcV03Container.XmlName);
        Container = rtpcNode.ReadRtpcV03Container();
        
        LoadClassDefinitions();
    }

    public void ToXml(string targetPath)
    {
        var parentIndexArrayOffset = Container.PropertyHeaders
            .First(h => h.NameHash == 0xCFD7B43E).RawData;
        var parentIndexArray = OvMaps.OffsetU32ArrayMap[BitConverter.ToUInt32(parentIndexArrayOffset)];
        Container.UnFlatten(parentIndexArray);
        
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

        xe.Write(Container, OvMaps, ClassDefinitions, true);
        
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
            var containerClassFile = Path.Join(classDirectory, $"{containerClass.ClassHash:X8}.xml");
            if (File.Exists(containerClassFile))
            {
                continue;
            }
        
            var xd = new XDocument();
            var xe = new XElement("Definition");
            xe.SetAttributeValue(nameof(containerClass.ClassHash), $"{containerClass.ClassHash:X8}");
            
            foreach (var member in containerClass.Members)
            {
                var mxe = new XElement("Member");
                mxe.SetAttributeValue(nameof(member.NameHash), $"{member.NameHash:X8}");
                mxe.SetAttributeValue(nameof(member.VariantType), $"{member.VariantType}");
                
                xe.Add(mxe);
            }
            
            xd.Add(xe);
            xe.Save(containerClassFile);
        }
    }
    
    protected static void LoadClassDefinitions()
    {
        var classDirectory = Settings.RtpcClassDirectory.Value;
        if (!Directory.Exists(classDirectory))
        {
            throw new DirectoryNotFoundException($"{nameof(Settings.RtpcClassDirectory)} not found: \"{classDirectory}\"");
        }

        var xmlFiles = Directory.GetFiles(classDirectory, "*.xml");
        foreach (var xmlFile in xmlFiles)
        {
            var xd = XDocument.Load(xmlFile);
        }
    }
}
