using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Utils;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct FRtpcV03ClassDefinitionMember
{
    public EVariantType VariantType = EVariantType.Unassigned;
    public uint NameHash = 0x0;
    
    public string NameHashHex = string.Empty;
    public string Name = string.Empty;

    public override string ToString() => $"{NameHash:X8}: {VariantType}";

    public FRtpcV03ClassDefinitionMember() {}

    public override int GetHashCode()
    {
        return HashCode.Combine(VariantType, NameHash);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is FRtpcV03ClassDefinitionMember other && Equals(other);
    }

    public bool Equals(FRtpcV03ClassDefinitionMember other)
    {
        return VariantType == other.VariantType &&
               NameHash == other.NameHash;
    }

    public static bool operator ==(FRtpcV03ClassDefinitionMember left, FRtpcV03ClassDefinitionMember right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FRtpcV03ClassDefinitionMember left, FRtpcV03ClassDefinitionMember right)
    {
        return !(left == right);
    }
}

public struct FRtpcV03ClassDefinition
{
    public uint ClassHash = 0x0;
    public List<FRtpcV03ClassDefinitionMember> Members = new();
    
    public string Name = string.Empty;

    public override string ToString() => $"{ClassHash:X8}: Member count {Members.Count}";

    public FRtpcV03ClassDefinition() {}

    public override int GetHashCode()
    {
        var classHashHash = ClassHash.GetHashCode();
        var membersHash = Members.Aggregate(0, (c, member) => c ^ member.GetHashCode());

        return classHashHash + membersHash;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is FRtpcV03ClassDefinition other && Equals(other);
    }

    public bool Equals(FRtpcV03ClassDefinition other)
    {
        return ClassHash == other.ClassHash && 
               Members.SequenceEqual(other.Members);
    }

    public static bool operator ==(FRtpcV03ClassDefinition left, FRtpcV03ClassDefinition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FRtpcV03ClassDefinition left, FRtpcV03ClassDefinition right)
    {
        return !(left == right);
    }
}

public static class FRtpcV03ClassExtensions
{
    public static readonly List<uint> DefaultMemberHashes = new()
    {
        ByteUtils.ReverseBytes(0xE65940D0), // Class hash
        ByteUtils.ReverseBytes(0x84B61AD3), // Name
        ByteUtils.ReverseBytes(0x8C863A7D), // Name hash
        ByteUtils.ReverseBytes(0x0584FFCF) // Object ID
    };
    
    public static IEnumerable<RtpcV03PropertyHeader> FilterDefaultMembers(IEnumerable<RtpcV03PropertyHeader> headers)
    {
        var filteredHeaders = headers.Where(h => !DefaultMemberHashes.Contains(h.NameHash));
        
        return filteredHeaders;
    }

    public static XElement CreateXElement(in this FRtpcV03ClassDefinition definition)
    {
        var xe = new XElement("Definition");
        xe.SetAttributeValue("ClassHash", $"{definition.ClassHash:X8}");
        
        if (!string.IsNullOrEmpty(definition.Name))
        {
            xe.SetAttributeValue(XElementExtensions.NameAttributeName, definition.Name);
        }
        
        foreach (var member in definition.Members)
        {
            var mxe = new XElement("Member");
            mxe.SetAttributeValue(nameof(member.NameHash), $"{member.NameHash:X8}");
            mxe.SetAttributeValue(nameof(member.VariantType), $"{member.VariantType.GetXmlName()}");
            mxe.SetAttributeValue(nameof(member.Name), $"{member.Name}");
            
            xe.Add(mxe);
        }

        return xe;
    }

    public static FRtpcV03ClassDefinition DefinitionFromXElement(this XElement xe)
    {
        var result = new FRtpcV03ClassDefinition();

        var classHashAttribute = xe.Attribute("ClassHash");
        if (classHashAttribute is null) throw new XmlSchemaException("ClassHash is missing from definition");
        result.ClassHash = uint.Parse(classHashAttribute.Value, NumberStyles.HexNumber);

        var mxeList = xe.Elements("Member");
        foreach (var mxe in mxeList)
        {
            var member = new FRtpcV03ClassDefinitionMember();

            var nameHashAttribute = mxe.Attribute(nameof(member.NameHash));
            if (nameHashAttribute is null) throw new XmlSchemaException($"{nameof(member.NameHash)} is missing from member");
            member.NameHash = uint.Parse(nameHashAttribute.Value, NumberStyles.HexNumber);

            var variantAttribute = mxe.Attribute(nameof(member.VariantType));
            if (variantAttribute is null) throw new XmlSchemaException($"{nameof(member.VariantType)} is missing from member");
            member.VariantType = EVariantTypeExtensions.GetVariant(variantAttribute.Value);
            
            result.Members.Add(member);
        }

        return result;
    }
}