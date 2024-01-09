namespace ApexTools.Core.Comparers;

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
