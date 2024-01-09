using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;
using ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public static class InlineVariantFactory
{
    public static readonly Dictionary<EVariantType, Func<IApexXElementIO>> VariantToClass = new()
    {
        { EVariantType.Unassigned, () => new InlineUnassigned() },
        { EVariantType.UInteger32, () => new InlineUInt32() },
        { EVariantType.Float32, () => new InlineFloat() },
        { EVariantType.String, () => new InlineString() },
        { EVariantType.Vector2, () => new InlineVector2() },
        { EVariantType.Vector3, () => new InlineVector3() },
        { EVariantType.Vector4, () => new InlineVector4() },
        { EVariantType.Matrix3X4, () => new InlineMatrix3X4() },
        { EVariantType.Events, () => new InlineEvents() },
    };
    
    public static IApexXElementIO VariantFromApex(this BinaryReader br)
    {
        var header = new InlinePropertyHeader();
        header.FromApex(br);

        IApexXElementIO property = header.VariantType switch
        {
            EVariantType.Unassigned => new InlineUnassigned(header),
            EVariantType.UInteger32 => new InlineUInt32(header),
            EVariantType.Float32 => new InlineFloat(header),
            EVariantType.String => new InlineString(header),
            EVariantType.Vector2 => new InlineVector2(header),
            EVariantType.Vector3 => new InlineVector3(header),
            EVariantType.Vector4 => new InlineVector4(header),
            EVariantType.Matrix3X4 => new InlineMatrix3X4(header),
            EVariantType.Events => new InlineEvents(header),
            _ => throw new ArgumentOutOfRangeException()
        };

        property.FromApex(br);

        return property;
    }
    
    public static IApexXElementIO VariantFromXElement(this XElement xe)
    {
        var variantType = VariantTypeExtensions.GetVariantType(xe.Name.ToString());
        var apexXElement = VariantToClass[variantType]();

        apexXElement.FromXElement(xe);

        return apexXElement;
    }
}