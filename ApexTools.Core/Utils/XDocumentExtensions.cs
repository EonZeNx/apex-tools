using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexTools.Core.Config;
using ApexTools.Core.Utils.Hash;

namespace ApexTools.Core.Utils;

public static class XDocumentExtensions
{
    public static void WriteNameOrHash(this XElement xe, uint nameHash, string name = "")
    {
        xe.WriteNameOrHash($"{nameHash:X8}", name);
    }
    
    public static void WriteNameOrHash(this XElement xe, string nameHash, string name = "")
    {
        if (Settings.AlwaysOutputHash.Value || string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue("NameHash", $"{nameHash}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue("Name", name);
        }
    }

    public static uint GetNameHash(this XElement xe)
    {
        var name = xe.Attribute("Name");
        if (name is not null)
        {
            var hash = name.Value.HashJenkins();
            return hash;
        }
        
        var nameHash = xe.Attribute("NameHash");
        if (nameHash is not null)
        {
            var hash = uint.Parse(nameHash.Value, NumberStyles.HexNumber);
            return hash;
        }

        throw new XmlSchemaException("Both Name and NameHash invalid");
    }
}