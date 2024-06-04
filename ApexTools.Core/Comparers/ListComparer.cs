using System.Numerics;

namespace ApexTools.Core.Comparers;

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