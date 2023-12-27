namespace ApexFormat.IRTPC.V01.Debug.Models.Properties.Variants;

public class Vec3 : F32Array
{
    public override string XmlName => "Vec3";
    public override EVariantType VariantType => EVariantType.Vec3;
    
    public override int Count { get; set; } = 3;
    
    
    public Vec3() { }
}