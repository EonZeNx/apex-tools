using System.Xml;
using ApexTools.RTPC.V03._1.Abstractions;
using ApexTools.RTPC.V03._1.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SContainerHeaderV03"/><br/>
/// Property headers - <see cref="SPropertyHeaderV03"/><br/>
/// Container headers - <see cref="SContainerHeaderV03"/><br/>
/// Valid property count - <see cref="uint"/>
/// </summary>
public class SContainerV03 : IFromApexHeader, IFromApex, IToXml
{
    public SContainerHeaderV03 Header = new();
    public SPropertyHeaderV03[] PropertyHeaders = Array.Empty<SPropertyHeaderV03>();
    public SPropertyV03[] Properties = Array.Empty<SPropertyV03>();
    public SContainerV03[] Containers = Array.Empty<SContainerV03>();
    public uint ValidPropertyCount = 0;

    public string XmlName => "Container";

    #region IFromApex

    public void FromApexHeader(BinaryReader br)
    {
        PropertyHeaders = new SPropertyHeaderV03[Header.PropertyCount];
        if (Header.PropertyCount != 0)
        {
            for (var i = 0; i < Header.PropertyCount; i++)
            {
                PropertyHeaders[i] = new SPropertyHeaderV03();
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
        Properties = new SPropertyV03[Header.PropertyCount];
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

    #endregion

    #region IToXml

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

    #endregion
}