using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec4 : FixedF32Array
{
    public override uint Count { get; set; } = 4;
    public override string XmlName => "Vec4";

    public VariantVec4()
    {
        Header.VariantType = EVariantType.Vector4;
    }
    public VariantVec4(PropertyHeaderV03 header) : base(header)
    { }
}