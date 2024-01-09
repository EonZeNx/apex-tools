using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexTools.Core.Interfaces;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public class InlineContainer : IFromApex, IToApex, ILookupHash
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
    public static InlineContainer FromXml(this XElement xe)
    {
        var result = new InlineContainer
        {
            NameHash = xe.GetNameHash()
        };
        
        var version01Attribute = xe.Attribute(nameof(result.Version01));
        if (version01Attribute is null)
        {
            throw new XmlSchemaException($"{nameof(result.Version01)} is invalid");
        }
        result.Version01 = byte.Parse(version01Attribute.Value);
        
        var version02Attribute = xe.Attribute(nameof(result.Version02));
        if (version02Attribute is null)
        {
            throw new XmlSchemaException($"{nameof(result.Version02)} is invalid");
        }
        result.Version02 = ushort.Parse(version02Attribute.Value);

        var propertyNames = VariantTypeExtensions.VariantToXml.Values.ToList();
        var propertyElements = xe.Elements()
            .Where(e => propertyNames.Contains(e.Name.ToString()))
            .ToList();

        result.PropertyCount = (ushort) propertyElements.Count;
        result.Properties = new IApexXElementIO[result.PropertyCount];
        for (var i = 0; i < result.PropertyCount; i++)
        {
            result.Properties[i] = propertyElements[i].VariantFromXElement();
        }

        return result;
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