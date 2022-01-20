using EonZeNx.ApexFormats.ADF.V04.Models;
using EonZeNx.ApexFormats.ADF.V04.Models.Types;
using EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants;

namespace EonZeNx.ApexFormats.ADF.V04.Utils;

public static class TypeDefUtils
{
    public static TypeDefV04 GetTypeDef(BinaryReader br)
    {
        var variant = new TypeDefV04();
        variant.LoadDefinitionFromApex(br);
        variant = variant.VariantType switch
        {
            EVariantType.Primitive => PrimitiveUtils.GetPrimitive(variant),
            EVariantType.Structure => (Structure) variant,
            EVariantType.Pointer => (Pointer) variant,
            EVariantType.Array => (AdfV04Array) variant,
            EVariantType.InlineArray => (InlineArray) variant,
            EVariantType.String => (AdfV04String) variant,
            EVariantType.Unknown => throw new ArgumentOutOfRangeException(),
            EVariantType.BitField => (BitField) variant,
            EVariantType.Enumeration => (Enumeration) variant,
            EVariantType.StringHash => (StringHash) variant,
            EVariantType.Deferred => (Deferred) variant,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        variant.LoadDefinitionFromApex(br);
        return variant;
    }
}