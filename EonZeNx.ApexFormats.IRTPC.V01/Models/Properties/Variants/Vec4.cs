namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Vec4 : IrtpcV01FloatArray
{
    public override string XmlName => "Vec4";
    protected override EVariantType VariantType => EVariantType.Vec4;
    
    public Vec4() { }
    public Vec4(IrtpcV01LoadProperty loadProperty) : base(loadProperty)
    {
        Num = 4;
    }
}