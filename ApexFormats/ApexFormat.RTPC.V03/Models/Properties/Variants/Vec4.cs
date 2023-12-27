namespace ApexFormat.RTPC.V03.Models.Properties.Variants;

public class Vec4 : FloatArray
{
    public override string XmlName => "Vec4";
    public override EVariantType VariantType => EVariantType.Vector4;
    public override int Alignment => 16;
    
    
    public override int Count { get; set; } = 4;
    
    
    public Vec4() { }
    public Vec4(PropertyHeaderV03 header) : base(header) { }
}