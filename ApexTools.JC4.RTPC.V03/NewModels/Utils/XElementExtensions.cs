using System.Globalization;
using System.Security;
using System.Xml.Linq;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.NewModels.Utils;

public static class XElementExtensions
{
    public static string NameHashAttributeName => "Hash";
    public static string NameAttributeName => "Name";
    
    public static void WriteNameOrHash(this XElement xe, uint nameHash, string name = "")
    {
        if (Settings.AlwaysOutputHash.Value || string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue(NameHashAttributeName, $"{nameHash:X8}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xe.SetAttributeValue(NameAttributeName, name);
        }
    }
    
    public static uint GetNameHash(this XElement xe)
    {
        var nameHashAttribute = xe.Attribute(NameHashAttributeName);
        if (nameHashAttribute is not null)
        {
            var nameHash = uint.Parse(nameHashAttribute.Value, NumberStyles.HexNumber);
            return nameHash;
        }
        
        var nameAttribute = xe.Attribute(NameAttributeName);
        if (nameAttribute is not null)
        {
            var nameHash = HashJenkinsL3.Hash(nameAttribute.Value);
            return nameHash;
        }

        throw new XmlSyntaxException($"Both {NameHashAttributeName} and {NameAttributeName} attributes missing from {xe}");
    }
    
    public static EVariantType GetVariant(this XElement xe)
    {
        var nodeName = xe.Name.ToString();
        return EVariantTypeExtensions.GetVariant(nodeName);
    }
}