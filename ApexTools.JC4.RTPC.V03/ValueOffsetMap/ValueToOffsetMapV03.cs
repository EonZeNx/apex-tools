using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.ValueOffsetMap;

// This got out of hand...
// I've been coding so long, I can't tell if this is genius or stupid

// ReSharper disable once InconsistentNaming
public class ValueToOffsetMapV03<T, U> where T : notnull
    where U : APropertyV03, IToApex, IGetValue<T>, new()
{
    public Dictionary<T, uint> Map;
    public readonly EVariantType Variant;
    public int Alignment;

    public ValueToOffsetMapV03(EVariantType variant, IEqualityComparer<T> comparer, int alignment = 4) : this(variant, alignment)
    {
        Map = new Dictionary<T, uint>(comparer);
    }

    public ValueToOffsetMapV03(EVariantType variant, int alignment = 4)
    {
        Map = new Dictionary<T, uint>();
        Variant = variant;
        Alignment = alignment;
    }

    public IEnumerable<U> Filter(IEnumerable<APropertyV03> properties)
    {
        return properties
            .Where(p => p.Header.VariantType == Variant)
            .OfType<U>()
            .GroupBy(p => p.GetValue())
            .Select(g => g.First());
    }

    public void Create(IEnumerable<APropertyV03> properties, BinaryWriter bw)
    {
        var unique = Filter(properties);
        foreach (var uniqueVariant in unique)
        {
            if (Map.ContainsKey(uniqueVariant.GetValue()))
            {
                continue;
            }
            
            bw.Align(Alignment);
            
            var offset = (uint) bw.Position();
            Map.Add(uniqueVariant.GetValue(), offset);
            uniqueVariant.ToApex(bw);
        }
    }
}