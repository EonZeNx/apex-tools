namespace ApexFormat.IRTPC.V01.Models.Properties.Variants;

public class Vec2 : FloatArray
{
    public override string XmlName => "Vec2";
    protected override EVariantType VariantType => EVariantType.Vec2;

    public Vec2() { }
    public Vec2(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01)
    {
        Num = 2;
    }
}