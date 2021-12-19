namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class Vec2 : FloatArray
{
    public override string XmlName => "Vec2";
    public override EVariantType VariantType => EVariantType.Vec2;
    
    public override int Count { get; set; } = 2;
    
    
    public Vec2() { }
    public Vec2(PropertyHeaderV01 header) : base(header) { }
}