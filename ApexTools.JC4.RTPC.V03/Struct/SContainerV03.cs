using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SContainerHeaderV03"/><br/>
/// Property headers - <see cref="JC4PropertyHeaderV03"/><br/>
/// Container headers - <see cref="SContainerHeaderV03"/><br/>
/// Valid property count - <see cref="uint"/>
/// </summary>
public class SContainerV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApexHeader, IToApex
{
    public SContainerHeaderV03 Header = new();
    public JC4PropertyHeaderV03[] PropertyHeaders = Array.Empty<JC4PropertyHeaderV03>();
    public APropertyV03[] Properties = Array.Empty<APropertyV03>();
    public SContainerV03[] Containers = Array.Empty<SContainerV03>();
    public uint ValidPropertyCount = 0;

    public string XmlName => "Container";

    public IEnumerable<APropertyV03> GetAllProperties()
    {
        var result = new List<APropertyV03>();
        result.AddRange(Properties);
        
        foreach (var container in Containers)
        {
            result.AddRange(container.GetAllProperties());
        }

        return result;
    }

    public IEnumerable<SContainerV03> GetAllContainers()
    {
        var result = new List<SContainerV03> { this };
        foreach (var container in Containers)
        {
            result.AddRange(container.GetAllContainers());
        }

        return result;
    }

    #region IApex

    public void FromApexHeader(BinaryReader br)
    {
        PropertyHeaders = new JC4PropertyHeaderV03[Header.PropertyCount];
        if (Header.PropertyCount != 0)
        {
            for (var i = 0; i < Header.PropertyCount; i++)
            {
                PropertyHeaders[i] = new JC4PropertyHeaderV03();
                PropertyHeaders[i].FromApexHeader(br);
            }
        }

        Containers = new SContainerV03[Header.ContainerCount];
        if (Header.ContainerCount != 0)
        {
            for (var i = 0; i < Header.ContainerCount; i++)
            {
                Containers[i] = new SContainerV03();
                Containers[i].Header.FromApexHeader(br);
            }
        }
        
        // Exclude unassigned values
        ValidPropertyCount = br.ReadUInt32();
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApexHeader(br);
        }
    }
    
    public void FromApex(BinaryReader br)
    {
        Properties = new APropertyV03[Header.PropertyCount];
        for (var i = 0; i < Properties.Length; i++)
        {
            var propertyHeader = PropertyHeaders[i];
            Properties[i] = propertyHeader.VariantType switch
            {
                EVariantType.UInteger32 => new VariantU32(propertyHeader),
                EVariantType.Float32 => new VariantF32(propertyHeader),
                EVariantType.String => new VariantStr(propertyHeader),
                EVariantType.Vec2 => new VariantVec2(propertyHeader),
                EVariantType.Vec3 => new VariantVec3(propertyHeader),
                EVariantType.Vec4 => new VariantVec4(propertyHeader),
                EVariantType.Mat3X3 => new VariantMat3X3(propertyHeader),
                EVariantType.Mat4X4 => new VariantMat4X4(propertyHeader),
                EVariantType.UInteger32Array => new VariantU32Array(propertyHeader),
                EVariantType.Float32Array => new VariantF32Array(propertyHeader),
                EVariantType.ByteArray => new VariantByteArray(propertyHeader),
                EVariantType.ObjectId => new VariantObjectId(propertyHeader),
                EVariantType.Event => new VariantEvent(propertyHeader),
                EVariantType.Unassigned => new VariantUnassigned(propertyHeader),
                EVariantType.Deprecated => new VariantUnassigned(propertyHeader),
                EVariantType.Total => new VariantUnassigned(propertyHeader),
                _ => throw new ArgumentOutOfRangeException()
            };

            Properties[i].FromApex(br);
        }
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApex(br);
        }
    }

    public void ToApexHeader(BinaryWriter bw)
    {
        for (var i = 0; i < Header.PropertyCount; i++)
        {
            PropertyHeaders[i].ToApexHeader(bw);
        }
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].Header.ToApexHeader(bw);
        }

        var validProperties = PropertyHeaders
            .Where(ph => ph.VariantType != EVariantType.Unassigned)
            .ToArray();
        bw.Write(validProperties.Length);
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].ToApexHeader(bw);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IXml

    public void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);

        foreach (var property in Properties)
        {
            property.ToXml(xw);
        }

        foreach (var container in Containers)
        {
            container.ToXml(xw);
        }
            
        xw.WriteEndElement();
    }

    public void FromXml(XmlReader xr)
    {
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);

        while (xr.Read())
        { if (xr.NodeType != XmlNodeType.Whitespace) break; }
        
        if (xr.Name != XmlName) PropertiesFromXml(xr);
            
        ContainersFromXml(xr);
    }

    #endregion
    
    #region XmlHelpers

    private void PropertiesFromXml(XmlReader xr)
    {
        var properties = new List<APropertyV03>();
        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;

            if (tag == XmlName && (nodeType is XmlNodeType.EndElement or XmlNodeType.Element)) break;
            if (nodeType != XmlNodeType.Element) continue;

            if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

            if (!XmlUtilsV03.XmlNameToBaseProperty.ContainsKey(tag))
            {
                throw new IOException($"Unknown property type: {tag}");
            }
            
            var property = XmlUtilsV03.XmlNameToBaseProperty[tag]();
            
            property.FromXml(xr);
            properties.Add(property);
        } while (xr.Read());

        Properties = properties.ToArray();
        Header.PropertyCount = (ushort) Properties.Length;
        PropertyHeaders = Properties.Select(p => p.Header).ToArray();
    }
    
    private void ContainersFromXml(XmlReader xr)
    {
        var containers = new List<SContainerV03>();

        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Container missing attributes");

            var container = new SContainerV03();

            container.FromXml(xr);
            containers.Add(container);
        } 
        while (xr.Read());

        Containers = containers.ToArray();
        Header.ContainerCount = (ushort) Containers.Length;
    }

    #endregion
}