using System.Xml.Linq;
using ApexFormat.RTPC.V03.Models.Properties;

namespace ApexFormat.RTPC.V03.Flat.Extensions;

public static class XElementExtensions
{
    public static EVariantType GetVariant(this XElement xe)
    {
        var nodeName = xe.Name.ToString();
        return EVariantTypeExtensions.GetVariant(nodeName);
    }
}