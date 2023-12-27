using System.Xml;
using ApexFormat.IRTPC.V01.Debug.Models.Properties;
using ApexFormat.IRTPC.V01.Debug.Models.Properties.Variants;
using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.IRTPC.V01.Debug.Models;

/// <summary>
/// The structure of an <see cref="PropertyContainer"/>.
/// <br/> Name hash - <see cref="int"/>
/// <br/> Version 01 - <see cref="byte"/>
/// <br/> Version 02 - <see cref="ushort"/>
/// <br/> Property count - <see cref="ushort"/>
/// <br/> <b>NOTE:</b> IRTPC containers only contain properties.
/// </summary>
public class PropertyContainer : XmlSerializable, IApexSerializable
{
    public override string XmlName => "PropertyContainer";
    
    public uint NameHash { get; set; }
    public byte Flag01 { get; set; }
    public ushort Flag02 { get; set; }
    public ushort PropertyCount { get; set; }
    public PropertyBase[] Properties { get; set; } = Array.Empty<PropertyBase>();
    
    public string Name { get; set; } = string.Empty;
    
    
    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        FromApex(br, false);
    }
    
    public void FromApex(BinaryReader br, bool notFirst)
    {
        if (!notFirst)
        {
            NameHash = br.ReadUInt32();
            Flag01 = br.ReadByte();
        }
        
        Flag02 = br.ReadUInt16();
        PropertyCount = br.ReadUInt16();
        
        // If valid connection, attempt hash lookup
        Name = HashUtils.Lookup(NameHash);
        
        PropertiesFromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(Flag01);
        bw.Write(Flag02);
        bw.Write(PropertyCount);
        
        foreach (var property in Properties)
        {
            property.ToApex(bw);
        }
    }

    
    #region ApexHelpers

    private void PropertiesFromApex(BinaryReader br)
    {
        Properties = new PropertyBase[PropertyCount];
        for (var i = 0; i < PropertyCount; i++)
        {
            var nameHash = br.ReadUInt32();
            var variantType = (EVariantType) br.ReadByte();
            PropertyBase property = variantType switch
            {
                EVariantType.UInteger32 => new UnsignedInt32(),
                EVariantType.Float32 => new F32(),
                EVariantType.String => new Str(),
                EVariantType.Vec2 => new Vec2(),
                EVariantType.Vec3 => new Vec3(),
                EVariantType.Vec4 => new Vec4(),
                EVariantType.Event => new Event(),
                EVariantType.Unassigned => throw new ArgumentOutOfRangeException(),
                EVariantType.Mat3X3 => new Mat3X3(),
                EVariantType.Mat3X4 => new Mat3X4(),
                EVariantType.UInteger32Array => new UInt32Array(),
                EVariantType.Float32Array => new F32Array(),
                EVariantType.ByteArray => new ByteArray(),
                EVariantType.Deprecated => throw new ArgumentOutOfRangeException(),
                EVariantType.ObjectId => new ObjectId(),
                EVariantType.Total => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };

            property.NameHash = nameHash;
            Properties[i] = property;
            property.FromApex(br);
        }
    }

    #endregion
    
    
    #endregion

    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Flag01 = byte.Parse(XmlUtils.GetAttribute(xr, nameof(Flag01)));
        Flag02 = ushort.Parse(XmlUtils.GetAttribute(xr, nameof(Flag02)));
        
        var properties = new List<PropertyBase>();
        xr.Read();

        while (xr.Read())
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

            if (!Utils.Utils.XmlNameToPropertyBase.ContainsKey(tag)) throw new IOException("Unknown property type");
            var property = Utils.Utils.XmlNameToPropertyBase[tag]();
            
            property.FromXml(xr);
            properties.Add(property);
        }

        Properties = properties.ToArray();
        PropertyCount = (ushort) Properties.Length;
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteAttributeString(nameof(Flag01), $"{Flag01}");
        xw.WriteAttributeString(nameof(Flag02), $"{Flag02}");
            
        foreach (var property in Properties)
        {
            property.ToXml(xw);
        }
        xw.WriteEndElement();
    }
    
    #endregion
}