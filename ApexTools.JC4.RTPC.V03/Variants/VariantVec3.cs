using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec3 : FixedF32Array
{
    public override uint Count { get; set; } = 3;
    public override string XmlName => "Vec3";

    public VariantVec3()
    {
        Header.VariantType = EVariantType.Vec3;
    }
    public VariantVec3(PropertyHeaderV03 header) : base(header)
    { }
}