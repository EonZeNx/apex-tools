namespace ApexTools.JC4.RTPC.V03.NewModels.Compare;

public class StringComparer : Comparer<string>
{
    public override int Compare(string? x, string? y)
    {
        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
        {
            return Default.Compare(x, y);
        }
        
        return Default.Compare(x, y);
    }
}