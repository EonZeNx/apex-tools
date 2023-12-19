using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantMat4X4 : FixedF32Array
{
    public override uint Count { get; set; } = 16;
    public override string XmlName => "Mat4";

    public VariantMat4X4()
    {
        Header.VariantType = EVariantType.Mat4X4;
    }
    public VariantMat4X4(JC4PropertyHeaderV03 header) : base(header)
    { }
}