using System.Xml;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace EonZeNx.ApexTools.Core.Utils;

public static class XmlUtils
{
    public static string GetAttribute(XmlReader xr, string attribute)
    {
        if (!xr.HasAttributes) throw new XmlException("Missing attributes");

        return xr.GetAttribute(attribute) ?? "";
    }
    
    public static void WriteNameOrNameHash(XmlWriter xw, int nameHash, string name = "")
    {
        if (Settings.AlwaysOutputHash.Value || string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("NameHash", $"{ByteUtils.ToHex(nameHash)}");
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            xw.WriteAttributeString("Name", name);
        }
    }
    
    public static int ReadNameIfValid(XmlReader xr)
    {
        var name = GetAttribute(xr, "Name");
        return name == ""
            ? ByteUtils.HexToInt(GetAttribute(xr, "NameHash"))
            : HashJenkinsL3.Hash(name);
    }
}