using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantMat4X4 : FixedF32Array
{
    public override uint Count { get; set; } = 16;
    public override string XmlName => "Mat4";

    public VariantMat4X4()
    {
        Header.VariantType = EVariantType.Matrix4X4;
    }
    public VariantMat4X4(PropertyHeaderV03 header) : base(header)
    { }
}