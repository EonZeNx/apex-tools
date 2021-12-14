namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Vec2 : IrtpcV01FloatArray
{
    public override string XmlName => "Vec2";
    protected override EVariantType VariantType => EVariantType.Vec2;

    public Vec2() { }
    public Vec2(IrtpcV01LoadProperty loadProperty) : base(loadProperty)
    {
        Num = 2;
    }
}