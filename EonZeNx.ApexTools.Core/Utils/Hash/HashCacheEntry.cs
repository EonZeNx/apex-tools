namespace EonZeNx.ApexTools.Core.Utils.Hash;

public class HashCacheEntry
{
    public readonly string Value;
    public int Count;

    public HashCacheEntry(string value)
    {
        Value = value;
        Count = 1;
    }
}