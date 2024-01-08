using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public class InlineContainer : IApexIO, ILookupHash
{
    public uint NameHash { get; set; }
    public byte Version01 { get; set; }
    public ushort Version02 { get; set; }
    public ushort PropertyCount { get; set; }

    public string Name { get; set; } = string.Empty;
    public IApexXElementIO[] Properties { get; set; } = Array.Empty<IApexXElementIO>();

    public static string XmlName => "Container";

    public void LookupHash()
    {
        Name = HashUtils.Lookup(NameHash);

        foreach (ref var property in Properties.AsSpan())
        {
            property.LookupHash();
        }
    }

    public void FromApex(BinaryReader br)
    {
        NameHash = br.ReadUInt32();
        Version01 = br.ReadByte();
        Version02 = br.ReadUInt16();
        PropertyCount = br.ReadUInt16();

        Properties = new IApexXElementIO[PropertyCount];
        for (var i = 0; i < PropertyCount; i++)
        {
            Properties[i] = br.VariantFromApex();
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(Version01);
        bw.Write(Version02);
        bw.Write(PropertyCount);
        
        foreach (var property in Properties)
        {
            property.ToApex(bw);
        }
    }
}

public static class InlineContainerExtensions
{
    public static void FromXml()
    {
        throw new NotImplementedException();
    }
    
    public static void ToXml(this XElement xe, InlineContainer container)
    {
        xe.WriteNameOrHash(container.NameHash, container.Name);
        xe.SetAttributeValue(nameof(container.Version01), container.Version01);
        xe.SetAttributeValue(nameof(container.Version02), container.Version02);
        
        foreach (ref var property in container.Properties.AsSpan())
        {
            xe.Add(property.ToXElement());
        }
    }

    public static void ToXml(this XElement pxe, InlineContainer[] containers)
    {
        foreach (ref var container in containers.AsSpan())
        {
            var xe = new XElement(InlineContainer.XmlName);
            xe.ToXml(container);
            
            pxe.Add(xe);
        }
    }
}