using System.Xml;
using ApexTools.Core.Config;
using ApexTools.Core.Exceptions;
using ApexTools.Core.Utils.Hash;

namespace ApexTools.Core.Utils;

public static class XmlUtils
{
    public static string GetAttribute(XmlReader xr, string attribute)
    {
        if (!xr.HasAttributes) throw new XmlException("Missing attributes");

        return xr.GetAttribute(attribute) ?? "";
    }
    
    public static void WriteNameOrNameHash(XmlWriter xw, string nameHash, string name = "")
    {
        if (Settings.AlwaysOutputHash.Value || string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("NameHash", $"{nameHash}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("Name", name);
        }
    }
    
    public static void WriteNameOrNameHash(XmlWriter xw, uint nameHash, string name = "")
    {
        if (Settings.AlwaysOutputHash.Value || string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("NameHash", $"{ByteUtils.ToHex(nameHash, true)}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("Name", name);
        }
    }
    
    public static uint ReadNameIfValid(XmlReader xr)
    {
        var name = GetAttribute(xr, "Name");
        if (!string.IsNullOrEmpty(name))
        {
            return ByteUtils.ReverseBytes(HashJenkinsL3.Hash(name));
        }
        
        var nameHash = GetAttribute(xr, "NameHash");
        if (!string.IsNullOrEmpty(nameHash))
        {
            return ByteUtils.HexToUInt(nameHash);
        }

        var xli = (IXmlLineInfo) xr;
        throw new MalformedXmlException($"Property does not have Name or NameHash (Line {xli.LineNumber})");
    }
}