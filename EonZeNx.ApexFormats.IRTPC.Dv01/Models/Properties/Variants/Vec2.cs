namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class Vec2 : F32Array
{
    public override string XmlName => "Vec2";
    public override EVariantType VariantType => EVariantType.Vec2;
    
    public override int Count { get; set; } = 2;
    
    
    public Vec2() { }
}