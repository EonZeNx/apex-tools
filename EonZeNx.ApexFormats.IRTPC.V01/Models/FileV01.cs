using System.Xml;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Exceptions;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models;

/// <summary>
/// The structure for <see cref="FileV01"/> file
/// <br/> Version 01 - <see cref="byte"/>
/// <br/> Version 02 - <see cref="ushort"/>
/// <br/> Object count - <see cref="ushort"/>
/// <br/> Root container - <see cref="ContainerV01"/>
/// </summary>
public class FileV01 : XmlSerializable, IApexFile, IApexSerializable
{
    public override string XmlName => "IRTPC";

    public EFourCc FourCc => EFourCc.Irtpc;
    public uint Version => 0x01;
    public ushort Version02 => 0x04;
    
    public ushort ObjectCount { get; set; }
    private ContainerV01[] Containers { get; set; } = Array.Empty<ContainerV01>();
    
    
    #region ApexSerializable
    
    public void FromApex(BinaryReader br)
    {
        if (br.ReadByte() != Version) throw new InvalidFileVersion();
        if (br.ReadUInt16() != Version02) throw new InvalidFileVersion();
        ObjectCount = br.ReadUInt16();
        
        Containers = new ContainerV01[ObjectCount];
        for (var i = 0; i < ObjectCount; i++)
        {
            Containers[i] = new ContainerV01();
            Containers[i].FromApex(br);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write((byte) Version);
        bw.Write(Version02);
        bw.Write(ObjectCount);
        foreach (var container in Containers)
        {
            container.ToApex(bw);
        }
    }
    
    #endregion

    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        var containers = new List<ContainerV01>();
        var container = new ContainerV01();
        
        xr.ReadToDescendant(container.XmlName);
        while (xr.NodeType == XmlNodeType.Element && xr.Name == container.XmlName)
        {
            container = new ContainerV01();
            container.FromXml(xr);
            containers.Add(container);
                
            xr.ReadToNextSibling(container.XmlName);
            if (xr.NodeType == XmlNodeType.EndElement) xr.ReadToNextSibling(container.XmlName);
        }
        xr.Close();

        Containers = containers.ToArray();
        ObjectCount = (ushort) Containers.Length;
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        foreach (var container in Containers)
        {
            container.ToXml(xw);
        }
            
        xw.WriteEndElement();
    }

    #endregion
}