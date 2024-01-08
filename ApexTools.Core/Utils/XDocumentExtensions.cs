using System.Xml.Linq;
using ApexTools.Core.Config;

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
}