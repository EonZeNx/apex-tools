namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties.Variants;

public class Vec2 : FloatArray
{
    public override string XmlName => "Vec2";
    public override EVariantType VariantType => EVariantType.Vector2;
    
    public override int Count { get; set; } = 2;
    
    
    public Vec2() { }
    public Vec2(PropertyHeaderV03 header) : base(header) { }
}