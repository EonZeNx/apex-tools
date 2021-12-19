using System.Xml;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Utils;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models;

/// <summary>
/// The structure of an <see cref="PropertyContainer"/>.
/// <br/> Name hash - <see cref="int"/>
/// <br/> Version 01 - <see cref="byte"/>
/// <br/> Version 02 - <see cref="ushort"/>
/// <br/> Property count - <see cref="ushort"/>
/// <br/> <b>NOTE:</b> IRTPC containers only contain properties.
/// </summary>
public class RootContainer : XmlSerializable, IApexSerializable
{
    public override string XmlName => "RootContainer";
    
    public int NameHash { get; set; }
    public byte Flag01 { get; set; }
    public ushort Flag02 { get; set; }
    public ushort PropertyContainerCount { get; set; }
    public PropertyContainer[] PropertyContainers { get; set; } = Array.Empty<PropertyContainer>();
    
    public string Name { get; set; } = string.Empty;
    
    
    #region ApexSerializable
    
    public void FromApex(BinaryReader br)
    {
        NameHash = br.ReadInt32();
        Flag01 = br.ReadByte();
        Flag02 = br.ReadUInt16();
        PropertyContainerCount = br.ReadUInt16();
        
        // If valid connection, attempt hash lookup
        Name = HashUtils.Lookup(NameHash);
        
        PropertyContainersFromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(Flag01);
        bw.Write(Flag02);
        bw.Write(PropertyContainerCount);
        
        foreach (var container in PropertyContainers)
        {
            container.ToApex(bw);
        }
    }

    
    #region ApexHelpers

    private void PropertyContainersFromApex(BinaryReader br)
    {
        var propertyContainers = new List<PropertyContainer>();

        for (var i = 0; i < PropertyContainerCount; i++)
        {
            var newContainer = new PropertyContainer();
            newContainer.FromApex(br);
            propertyContainers.Add(newContainer);
        }
        
        while (br.Position() < br.BaseStream.Length)
        {
            var newContainer = new PropertyContainer();
            newContainer.FromApex(br, propertyContainers.Count > 0);
            propertyContainers.Add(newContainer);
        }
        
        PropertyContainers = propertyContainers.ToArray();
    }

    #endregion
    
    
    #endregion

    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        var containers = new List<PropertyContainer>();

        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

            var container = new PropertyContainer();

            container.FromXml(xr);
            containers.Add(container);
        } 
        while (xr.Read());

        PropertyContainers = containers.ToArray();
        PropertyContainerCount = (ushort) PropertyContainers.Length;
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteAttributeString(nameof(Flag01), $"{Flag01}");
        xw.WriteAttributeString(nameof(Flag02), $"{Flag02}");
            
        foreach (var property in PropertyContainers)
        {
            property.ToXml(xw);
        }
        xw.WriteEndElement();
    }
    
    #endregion
}