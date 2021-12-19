namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Vec4 : FloatArray
{
    public override string XmlName => "Vec4";
    protected override EVariantType VariantType => EVariantType.Vec4;
    
    public Vec4() { }
    public Vec4(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01)
    {
        Num = 4;
    }
}