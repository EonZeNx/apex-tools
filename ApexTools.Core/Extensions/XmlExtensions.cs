using System.Xml;
using System.Xml.Schema;
using ApexTools.Core.Config;
using ApexTools.Core.Hash;
using ApexTools.Core.Utils;

namespace ApexTools.Core.Extensions;

public static class XmlExtensions
{
    public static string GetAttribute(this XmlReader xr, string attribute)
    {
        if (!xr.HasAttributes) throw new XmlException("Missing attributes");

        return xr.GetAttribute(attribute) ?? "";
    }
    
    public static void WriteNameOrNameHash(this XmlWriter xw, string nameHash, string name = "")
    {
        if (Settings.OutputNameHash.Value || string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("NameHash", $"{nameHash}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("Name", name);
        }
    }
    
    public static void WriteNameOrNameHash(this XmlWriter xw, uint nameHash, string name = "")
    {
        if (Settings.OutputNameHash.Value || string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("NameHash", $"{ByteUtils.ToHex(nameHash, true)}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("Name", name);
        }
    }
    
    public static uint ReadNameIfValid(this XmlReader xr)
    {
        var name = GetAttribute(xr, "Name");
        if (!string.IsNullOrEmpty(name))
        {
            return name.HashJenkins().LittleEndian();
        }
        
        var nameHash = GetAttribute(xr, "NameHash");
        if (!string.IsNullOrEmpty(nameHash))
        {
            return ByteUtils.HexToUInt(nameHash);
        }

        var xli = (IXmlLineInfo) xr;
        throw new XmlSchemaException($"Property does not have Name or NameHash (Line {xli.LineNumber})");
    }
}