using System.Numerics;

namespace ApexTools.JC4.RTPC.V03;

public static class MathUtilsV03
{
    public static bool F32ArrayEqual(float[] a, float[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        return !a.Where((t, i) => Math.Abs(t - b[i]) > 0.0001f).Any();
    }
}

public class ListEqualityComparer<T> : IEqualityComparer<IList<T>>
{
    public bool Equals(IList<T>? x, IList<T>? y)
    {
        return x != null && y != null && 
               x.SequenceEqual(y);
    }

    public int GetHashCode(IList<T> list)
    {
        var hash = 0;
        if (ReferenceEquals(list, null) || list.Count == 0) return hash;

        return (from o in list
            where o is not null
            select o.GetHashCode())
            .Aggregate(hash, (current, h) => current == 0 ? h : current ^ h);
    }
}

public class U64BComparer : IEqualityComparer<(ulong, byte)>
{
    public bool Equals((ulong, byte) x, (ulong, byte) y)
    {
        return x.Item1 == y.Item1 && 
               x.Item2 == y.Item2;
    }

    public int GetHashCode((ulong, byte) tuple)
    {
        return tuple.Item1.GetHashCode() ^ tuple.Item2.GetHashCode();
    }
}

public class ListComparer<T> : Comparer<IList<T>> where T : INumber<T>
{
    public override int Compare(IList<T>? x, IList<T>? y)
    {
        if (x is null) return 1;
        if (y is null) return -1;

        if (x.Count != y.Count)
        {
            return x.Count < y.Count ? -1 : 1;
        }

        for (var i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i])
            {
                return x[i] < y[i] ? -1 : 1;
            }
        }

        return 0;
    }
}