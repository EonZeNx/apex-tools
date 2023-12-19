using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantVec4 : FixedF32Array
{
    public override uint Count { get; set; } = 4;
    public override string XmlName => "Vec4";

    public VariantVec4()
    {
        Header.VariantType = EVariantType.Vec4;
    }
    public VariantVec4(JC4PropertyHeaderV03 header) : base(header)
    { }
}