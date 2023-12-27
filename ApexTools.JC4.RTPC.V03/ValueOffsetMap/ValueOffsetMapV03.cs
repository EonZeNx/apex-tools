using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.ValueOffsetMap;

// This got out of hand...
// I've been coding so long, I can't tell if this is genius or stupid

// ReSharper disable once InconsistentNaming
public class ValueOffsetMapV03<T, U> where T : notnull
    where U : APropertyV03, IToApex, IGetValue<T>, new()
{
    public Dictionary<T, uint> Map;
    public readonly EVariantType Variant;
    public int Alignment;

    public ValueOffsetMapV03(EVariantType variant, IEqualityComparer<T> comparer, int alignment = 4) : this(variant, alignment)
    {
        Map = new Dictionary<T, uint>(comparer);
    }

    public ValueOffsetMapV03(EVariantType variant, int alignment = 4)
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

    public IEnumerable<U> Sort(IEnumerable<APropertyV03> properties)
    {
        if (typeof(T).GetInterfaces().Contains(typeof(IComparable)))
        {
            return properties
                .OfType<U>()
                .OrderBy(p => p.GetValue());
        }

        return properties.OfType<U>();
    }

    public void Create(IEnumerable<APropertyV03> properties, BinaryWriter bw)
    {
        var variantProperties = properties.Where(p => p.Header.VariantType == Variant);
        var unique = Filter(variantProperties);
        var sortedUnique = Sort(unique);
        
        foreach (var uniqueVariant in sortedUnique)
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