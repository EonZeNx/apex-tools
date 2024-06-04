namespace ApexTools.Core;

public class ApexValue<T> where T : notnull
{
    public T Value { get; }
    public string Name { get; }

    public ApexValue(T value, string name)
    {
        Value = value;
        Name = name;
    }
}
