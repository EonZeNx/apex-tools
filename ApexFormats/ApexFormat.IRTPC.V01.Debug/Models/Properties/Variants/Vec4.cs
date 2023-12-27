namespace ApexFormat.IRTPC.V01.Debug.Models.Properties.Variants;

public class Vec4 : F32Array
{
    public override string XmlName => "Vec4";
    public override EVariantType VariantType => EVariantType.Vec4;
    
    
    public override int Count { get; set; } = 4;
    
    
    public Vec4() { }
}