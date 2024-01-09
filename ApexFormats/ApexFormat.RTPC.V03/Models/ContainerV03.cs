using System.ComponentModel;
using System.Xml;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexFormat.RTPC.V03.Models.Properties.Variants;
using ApexFormat.RTPC.V03.Utils;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Abstractions.Serializable;
using ApexTools.Core.Config;
using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models;

/// <summary>
/// The structure of an <see cref="ContainerV03"/> file
/// <br/> Name hash - <see cref="int"/>
/// <br/> Offset - <see cref="uint"/>
/// <br/> Property count - <see cref="ushort"/>
/// <br/> Container count - <see cref="ushort"/>
/// <br/> Property headers
/// <br/> Containers headers
/// <br/> Properties - <see cref="PropertyBaseV03"/>[]
/// <br/> Containers - <see cref="ContainerV03"/>[]
/// </summary>
public class ContainerV03 : XmlSerializable, IApexSerializable, IFromApexHeaderSerializable, IToApexSerializableDeferred
{
    public override string XmlName => "Container";
    public static int HeaderSize => 4 + 4 + 2 + 2;
    
    public uint NameHash { get; set; }
    public string HexNameHash => ByteUtils.ToHex(NameHash);
    public string Name { get; set; } = string.Empty;
    public uint Offset { get; set; }
    public ushort PropertyCount { get; set; }
    public ushort ContainerCount { get; set; }
    
    public long ContainerHeaderOffset { get; set; }
    public long ContainerHeaderStart { get; set; }
    public long DataOffset { get; set; }
    
    public PropertyBaseV03[] Properties { get; set; } = Array.Empty<PropertyBaseV03>();
    public ContainerV03[] Containers { get; set; } = Array.Empty<ContainerV03>();
    
    
    #region ApexSerializable

    public void FromApexHeader(BinaryReader br)
    {
        // Read variables
        NameHash = br.ReadUInt32();
        Offset = br.ReadUInt32();
        PropertyCount = br.ReadUInt16();
        ContainerCount = br.ReadUInt16();
    }

    public void FromApex(BinaryReader br)
    {
        // Read properties and sub-containers
        PropertiesFromApex(br);
        ContainersFromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(Offset);
        bw.Write(PropertyCount);
        bw.Write(ContainerCount);
    }

    public void ToApexDeferred(BinaryWriter bw)
    {
        Offset = (uint) bw.Position();
        ContainerHeaderStart = ByteUtils.Align(Offset + PropertyCount * PropertyBaseV03.HeaderSize, 4);
        DataOffset = ContainerHeaderStart + ContainerCount * HeaderSize;

        if (Properties.Length > 0) PropertiesToApex(bw);
        if (Containers.Length > 0) ContainersToApex(bw);

        bw.Seek((int) DataOffset, SeekOrigin.Begin);
    }

    
    #region FromApex Utils

    private void PropertiesFromApex(BinaryReader br)
    {
        br.BaseStream.Seek(Offset, SeekOrigin.Begin);
        
        var propertyHeaders = new PropertyHeaderV03[PropertyCount];
        for (var i = 0; i < propertyHeaders.Length; i++)
        {
            propertyHeaders[i] = new PropertyHeaderV03(br);
        }

        ContainerHeaderOffset = br.Position();
        Properties = new PropertyBaseV03[PropertyCount];
        for (var i = 0; i < PropertyCount; i++)
        {
            var header = propertyHeaders[i];
            Properties[i] = header.VariantType switch
            {
                EVariantType.Unassigned => new Unassigned(header),
                EVariantType.UInteger32 => new UnsignedInt32(header),
                EVariantType.Float32 => new F32(header),
                EVariantType.String => new Str(header),
                EVariantType.Vector2 => new Vec2(header),
                EVariantType.Vector3 => new Vec3(header),
                EVariantType.Vector4 => new Vec4(header),
                EVariantType.Matrix3X3 => new Mat3X3(header),
                EVariantType.Matrix4X4 => new Mat4X4(header),
                EVariantType.UInteger32Array => new UInt32Array(header),
                EVariantType.Float32Array => new FloatArray(header),
                EVariantType.ByteArray => new ByteArray(header),
                EVariantType.Deprecated => throw new InvalidEnumArgumentException($"RTPC v01 variant type is '{header.VariantType}'"),
                EVariantType.ObjectId => new ObjectId(header),
                EVariantType.Event => new Event(header),
                EVariantType.Total => throw new InvalidEnumArgumentException($"RTPC v01 variant type is '{header.VariantType}'"),
                _ => throw new ArgumentOutOfRangeException($"RTPC v01 variant type is '{header.VariantType}'")
            };
            
            Properties[i].FromApex(br);
        }

        SortProperties();
    }
    
    private void ContainersFromApex(BinaryReader br)
    {
        br.BaseStream.Seek(ContainerHeaderOffset, SeekOrigin.Begin);
        br.Align(4);
        Containers = new ContainerV03[ContainerCount];
        for (var i = 0; i < ContainerCount; i++)
        {
            Containers[i] = new ContainerV03();
            Containers[i].FromApexHeader(br);
        }
        
        for (var i = 0; i < ContainerCount; i++)
        {
            Containers[i].FromApex(br);
        }
    }

    #endregion


    #region ToApex Utils

    private void PropertiesToApex(BinaryWriter bw)
    {
        bw.Seek((int) DataOffset, SeekOrigin.Begin);
            
        foreach (var property in Properties)
        {
            if (property is IToApexSerializableDeferred propertyDeferred)
            {
                propertyDeferred.ToApexDeferred(bw);
            }
        }

        bw.Align(4);
        DataOffset = bw.Position();

        bw.Seek((int) Offset, SeekOrigin.Begin);
        foreach (var property in Properties)
        {
            property.ToApex(bw);
        }
            
        // TODO: On final property in file, fix this to not align
        bw.Align(4);
    }
    
    private void ContainersToApex(BinaryWriter bw)
    {
        bw.Seek((int) DataOffset, SeekOrigin.Begin);
            
        foreach (var container in Containers)
        {
            container.ToApexDeferred(bw);
        }

        DataOffset = bw.Position();
        bw.Seek((int) ContainerHeaderStart, SeekOrigin.Begin);
        bw.Align(4);
            
        foreach (var container in Containers)
        {
            container.ToApex(bw);
        }
    }

    #endregion
    
    
    #region Utils

    public void SortProperties()
    {
        // Sort properties using NameHash
        if (Settings.RtpcSortProperties.Value)
        {
            Array.Sort(Properties, new PropertyV03Comparer());
        }
    }

    #endregion


    #endregion
    
    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = xr.ReadNameIfValid();

        while (xr.Read())
        { if (xr.NodeType != XmlNodeType.Whitespace) break; }
            
        if (xr.Name != XmlName) PropertiesFromXml(xr);
            
        ContainersFromXml(xr);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        xw.WriteNameOrNameHash(HexNameHash, Name);
        
        foreach (var property in Properties) property.ToXml(xw);
        foreach (var container in Containers) container.ToXml(xw);
            
        xw.WriteEndElement();
    }

    #region XmlHelpers

    private void PropertiesFromXml(XmlReader xr)
    {
        var properties = new List<PropertyBaseV03>();
        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;

            if (tag == XmlName && (nodeType is XmlNodeType.EndElement or XmlNodeType.Element)) break;
            if (nodeType != XmlNodeType.Element) continue;

            if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

            if (!UtilsV03.XmlNameToBaseProperty.ContainsKey(tag))
                throw new IOException($"Unknown property type: {tag}");
            var property = UtilsV03.XmlNameToBaseProperty[tag]();

            property.FromXml(xr);
            properties.Add(property);
        } while (xr.Read());

        Properties = properties.ToArray();
        PropertyCount = (ushort) Properties.Length;
    }
    
    private void ContainersFromXml(XmlReader xr)
    {
        var containers = new List<ContainerV03>();

        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

            var container = new ContainerV03();

            container.FromXml(xr);
            containers.Add(container);
        } 
        while (xr.Read());

        Containers = containers.ToArray();
        ContainerCount = (ushort) Containers.Length;
    }

    #endregion
    
    #endregion
}