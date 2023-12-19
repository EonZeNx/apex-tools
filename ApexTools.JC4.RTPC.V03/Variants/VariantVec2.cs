using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec2 : FixedF32Array
{
    public override uint Count { get; set; } = 2;
    public override string XmlName => "Vec2";

    public VariantVec2()
    {
        Header.VariantType = EVariantType.Vec2;
    }
    public VariantVec2(JC4PropertyHeaderV03 header) : base(header)
    { }
}