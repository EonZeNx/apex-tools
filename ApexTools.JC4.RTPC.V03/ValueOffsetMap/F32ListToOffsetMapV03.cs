using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.ValueOffsetMap;

// ReSharper disable once InconsistentNaming
public class F32ListToOffsetMapV03<U> : ValueToOffsetMapV03<IList<float>, U>
    where U : APropertyV03, IToApex, IGetValue<IList<float>>, new()
{
    public F32ListToOffsetMapV03(EVariantType variant, int alignment = 4) : base(variant, alignment)
    {
        var comparer = new ListComparer<float>();
        Map = new Dictionary<IList<float>, uint>(comparer);
    }
}