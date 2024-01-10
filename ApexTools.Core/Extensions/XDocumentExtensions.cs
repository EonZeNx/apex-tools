using System.Globalization;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using ApexTools.Core.Config;
using ApexTools.Core.Hash;

namespace ApexTools.Core.Extensions;

public static class XDocumentExtensions
{
    public static string NameHashAttributeString => "Hash";
    public static string NameAttributeString => "Name";
    
    public static void WriteNameOrHash(this XElement xe, uint nameHash, string name = "")
    {
        xe.WriteNameOrHash($"{nameHash:X8}", name);
    }
    
    public static void WriteNameOrHash(this XElement xe, string nameHash, string name = "")
    {
        if (Settings.OutputNameHash.Value || string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue(NameHashAttributeString, $"{nameHash}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue(NameAttributeString, name);
        }
    }
    
    public static string GetAttribute(this XElement xe, string name)
    {
        var attribute = xe.Attribute(name);
        if (attribute is null) throw new XmlException("Missing attributes");

        return attribute.Value;
    }
    
    public static string TryGetAttribute(this XElement xe, string name)
    {
        var attribute = xe.Attribute(name);

        return attribute?.Value ?? string.Empty;
    }

    public static uint GetNameHash(this XElement xe)
    {
        var name = xe.Attribute(NameAttributeString);
        if (name is not null)
        {
            var hash = name.Value.HashJenkins();
            return hash;
        }
        
        var nameHash = xe.Attribute(NameHashAttributeString);
        if (nameHash is not null)
        {
            var hash = uint.Parse(nameHash.Value, NumberStyles.HexNumber);
            return hash;
        }
        var nodePosition = xe.ElementsBeforeSelf().Count();
        throw new XmlSyntaxException($"Both {NameHashAttributeString} and {NameAttributeString} attributes missing from node #{nodePosition}");
    }
}