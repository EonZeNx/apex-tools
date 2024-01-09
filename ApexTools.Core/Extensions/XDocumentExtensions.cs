using System.Globalization;
using System.Security;
using System.Xml.Linq;
using ApexTools.Core.Config;
using ApexTools.Hash;

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