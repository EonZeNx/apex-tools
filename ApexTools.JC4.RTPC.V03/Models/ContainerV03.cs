﻿using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Models;

/// <summary>
/// Format:<br/>
/// Header - <see cref="ContainerHeaderV03"/><br/>
/// Property headers - <see cref="PropertyHeaderV03"/><br/>
/// Container headers - <see cref="ContainerHeaderV03"/><br/>
/// Valid property count - <see cref="uint"/>
/// </summary>
public class ContainerV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApexHeader, IToApex
{
    public ContainerHeaderV03 Header = new();
    public PropertyHeaderV03[] PropertyHeaders = Array.Empty<PropertyHeaderV03>();
    public APropertyV03[] Properties = Array.Empty<APropertyV03>();
    public ContainerV03[] Containers = Array.Empty<ContainerV03>();
    public uint ValidPropertyCount = 0;
    
    public bool Flat = false;

    public virtual string XmlName => "Container";
    
    public IEnumerable<PropertyHeaderV03> GetAllPropertyHeaders()
    {
        var result = new List<PropertyHeaderV03>();
        result.AddRange(PropertyHeaders);
        
        foreach (var container in Containers)
        {
            result.AddRange(container.GetAllPropertyHeaders());
        }

        return result;
    }

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

    public IEnumerable<ContainerV03> GetAllContainers()
    {
        var result = new List<ContainerV03> { this };
        foreach (var container in Containers)
        {
            result.AddRange(container.GetAllContainers());
        }

        return result;
    }
    
    public ulong GetObjectId(uint hash)
    {
        var hasObjectId = Properties.Any(p => p.Header.NameHash == hash);

        if (!hasObjectId) return 0;
        var objectIdVariant = (VariantObjectId) Properties.First(p => p.Header.NameHash == hash);
        var objectId = objectIdVariant.Value.Item1;

        return objectId;
    }
    
    public IEnumerable<ContainerV03> GetAllContainersFlat()
    {
        var result = new List<ContainerV03>();
        if (Flat) result.Add(this);

        foreach (var container in Containers)
        {
            var containersFlat = container.GetAllContainersFlat();
            result.AddRange(containersFlat);
        }
        
        Containers = Containers.Where(c => !c.Flat).ToArray();
        Header.ContainerCount = (ushort) Containers.Length;

        return result;
    }

    public uint GetContainerCount()
    {
        var count = (uint) Containers.Length;
        foreach (var container in Containers)
        {
            count += container.GetContainerCount();
        }

        return count;
    }
    
    public uint GetContainerCountFlat()
    {
        uint count = 0;
        foreach (var container in Containers)
        {
            if (container.Flat) count += 1;
            count += container.GetContainerCountFlat();
        }

        return count;
    }

    #region IApex

    public void FromApexHeader(BinaryReader br)
    {
        PropertyHeaders = new PropertyHeaderV03[Header.PropertyCount];
        if (Header.PropertyCount != 0)
        {
            for (var i = 0; i < Header.PropertyCount; i++)
            {
                PropertyHeaders[i] = new PropertyHeaderV03();
                PropertyHeaders[i].FromApex(br);
            }
        }

        Containers = new ContainerV03[Header.ContainerCount];
        if (Header.ContainerCount != 0)
        {
            for (var i = 0; i < Header.ContainerCount; i++)
            {
                Containers[i] = new ContainerV03();
                Containers[i].Header.FromApex(br);
            }
        }
        
        // Exclude unassigned values
        ValidPropertyCount = br.ReadUInt32();
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApexHeader(br);
        }
    }
    
    public virtual void FromApex(BinaryReader br)
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
                EVariantType.Vector2 => new VariantVec2(propertyHeader),
                EVariantType.Vector3 => new VariantVec3(propertyHeader),
                EVariantType.Vector4 => new VariantVec4(propertyHeader),
                EVariantType.Matrix3X3 => new VariantMat3X3(propertyHeader),
                EVariantType.Matrix4X4 => new VariantMat4X4(propertyHeader),
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

    public virtual void ToApexHeader(BinaryWriter bw)
    {
        for (var i = 0; i < Header.PropertyCount; i++)
        {
            PropertyHeaders[i].ToApex(bw);
        }
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].Header.ToApex(bw);
        }

        var validProperties = PropertyHeaders
            .Count(ph => ph.VariantType != EVariantType.Unassigned);
        bw.Write(validProperties);
        
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

    public virtual void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);
        
        xw.WriteAttributeString("Flat", $"{Flat}");

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

    public virtual void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
        Flat = bool.Parse(xr.GetAttribute(nameof(Flat)) ?? $"{false}");

        while (xr.Read())
        { if (xr.NodeType != XmlNodeType.Whitespace) break; }
        
        if (xr.Name != XmlName) PropertiesFromXml(xr);
            
        ContainersFromXml(xr);
    }

    #endregion
    
    #region XmlHelpers

    protected void PropertiesFromXml(XmlReader xr)
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
    
    protected void ContainersFromXml(XmlReader xr)
    {
        var containers = new List<ContainerV03>();

        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Container missing attributes");

            var container = new ContainerV03();

            container.FromXml(xr);
            containers.Add(container);
        } 
        while (xr.Read());

        Containers = containers.ToArray();
        Header.ContainerCount = (ushort) Containers.Length;
    }

    #endregion
}