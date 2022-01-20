
namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants;

public abstract class PrimitiveBase<T> : TypeDefV04
{
    public PrimitiveBase()
    {
        VariantType = EVariantType.Primitive;
    }

    public abstract T Value { get; set; }
}