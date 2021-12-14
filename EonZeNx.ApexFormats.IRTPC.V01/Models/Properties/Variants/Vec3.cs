namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Vec3 : IrtpcV01FloatArray
{
    public override string XmlName => "Vec3";
    protected override EVariantType VariantType => EVariantType.Vec3;
    
    public Vec3() { }
    public Vec3(IrtpcV01LoadProperty loadProperty) : base(loadProperty)
    {
        Num = 3;
    }
}