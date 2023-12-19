using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec3 : FixedF32Array
{
    public override uint Count { get; set; } = 3;
    public override string XmlName => "Vec3";

    public VariantVec3()
    {
        Header.VariantType = EVariantType.Vec3;
    }
    public VariantVec3(JC4PropertyHeaderV03 header) : base(header)
    { }
}