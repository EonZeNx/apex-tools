namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class Vec4 : F32Array
{
    public override string XmlName => "Vec4";
    public override EVariantType VariantType => EVariantType.Vec4;
    public override int Alignment => 16;
    
    
    public override int Count { get; set; } = 4;
    
    
    public Vec4() { }
    public Vec4(RtpcV01PropertyHeader header) : base(header) { }
}