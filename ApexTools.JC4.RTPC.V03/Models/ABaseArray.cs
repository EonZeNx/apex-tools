using ApexTools.JC4.RTPC.V03.Abstractions;

namespace ApexTools.JC4.RTPC.V03.Models;

public abstract class ABaseArray<T> : APropertyV03, IGetValue<IList<T>>
{
    public abstract List<T> Value { get; set; }
    public abstract uint Count { get; set; }

    public ABaseArray() { }
    public ABaseArray(PropertyHeaderV03 header) : base(header)
    { }

    public IList<T> GetValue()
    {
        return Value;
    }
}