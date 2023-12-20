using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantMat3X3 : FixedF32Array
{
    public override uint Count { get; set; } = 9;
    public override string XmlName => "Mat3";

    public VariantMat3X3()
    {
        Header.VariantType = EVariantType.Mat3X3;
    }
    public VariantMat3X3(PropertyHeaderV03 header) : base(header)
    { }
}