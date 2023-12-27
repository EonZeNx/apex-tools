using ApexFormat.ADF.V04.Models;
using ApexFormat.ADF.V04.Models.Types;
using ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

namespace ApexFormat.ADF.V04.Utils;

public static class PrimitiveUtils
{
    public static TypeDefV04 GetPrimitive(TypeDefV04 variant)
    {
        return variant.ScalarType switch
        {
            EScalarType.Signed => variant.Size switch
            {
                1 => new PrimitiveSByte(),
                2 => new PrimitiveShort(),
                4 => new PrimitiveInt(),
                8 => new PrimitiveLong(),
                _ => throw new ArgumentOutOfRangeException(nameof(variant.Size), variant.Size, null)
            },
            EScalarType.Unsigned => variant.Size switch
            {
                1 => new PrimitiveByte(),
                2 => new PrimitiveUShort(),
                4 => new PrimitiveUInt(),
                8 => new PrimitiveULong(),
                _ => throw new ArgumentOutOfRangeException(nameof(variant.Size), variant.Size, null)
            },
            EScalarType.Float => variant.Size switch
            {
                4 => new PrimitiveFloat(),
                8 => new PrimitiveDouble(),
                _ => throw new ArgumentOutOfRangeException(nameof(variant.Size), variant.Size, null)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(variant.ScalarType), variant.ScalarType, null)
        };
    }
}