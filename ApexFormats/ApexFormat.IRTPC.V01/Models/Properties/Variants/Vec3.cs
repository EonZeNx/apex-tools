namespace ApexFormat.IRTPC.V01.Models.Properties.Variants;

public class Vec3 : FloatArray
{
    public override string XmlName => "Vec3";
    protected override EVariantType VariantType => EVariantType.Vec3;
    
    public Vec3() { }
    public Vec3(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01)
    {
        Num = 3;
    }
}