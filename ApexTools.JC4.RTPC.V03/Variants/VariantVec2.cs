using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec2 : FixedF32Array
{
    public override uint Count { get; set; } = 2;
    public override string XmlName => "Vec2";

    public VariantVec2()
    {
        Header.VariantType = EVariantType.Vec2;
    }
    public VariantVec2(PropertyHeaderV03 header) : base(header)
    { }
}