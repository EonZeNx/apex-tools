namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Vec3 : IrtpcV01FloatArray
{
    public override string XmlName => "Vec3";
    protected override EVariantType VariantType => EVariantType.Vec3;
    
    public Vec3() { }
    public Vec3(IrtpcV01PropertyHeader propertyHeader) : base(propertyHeader)
    {
        Num = 3;
    }
}