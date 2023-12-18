namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties.Variants;

public class Vec3 : FloatArray
{
    public override string XmlName => "Vec3";
    public override EVariantType VariantType => EVariantType.Vec3;
    
    public override int Count { get; set; } = 3;
    
    
    public Vec3() { }
    public Vec3(PropertyHeaderV03 header) : base(header) { }
}