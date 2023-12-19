using ApexTools.RTPC.V03._1.Struct;

namespace ApexTools.RTPC.V03._1;

public abstract class BaseArray<T> : SPropertyV03
{
    public abstract T[] Value { get; set; }
    public abstract uint Count { get; set; }

    public BaseArray() { }
    public BaseArray(SPropertyHeaderV03 header) : base(header)
    { }
}