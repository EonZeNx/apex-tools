using EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

namespace EonZeNx.ApexFormats.RTPC.V01.Models;

public class RtpcV01PropertyComparer : Comparer<RtpcV01PropertyBase>
{
    public override int Compare(RtpcV01PropertyBase? x, RtpcV01PropertyBase? y)
    {
        // Null compare
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        if (string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(y.Name))
            return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.CurrentCulture);
        
        // Name null compare
        if (string.IsNullOrWhiteSpace(x.Name)) return 1;
        if (string.IsNullOrWhiteSpace(y.Name)) return -1;

        // Name compare
        return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
    }
}

public class RtpcV01ContainerComparer : Comparer<RtpcV01Container>
{
    public override int Compare(RtpcV01Container? x, RtpcV01Container? y)
    {
        // Null compare
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        if (string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(y.Name))
            return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.CurrentCulture);
        
        // Name null compare
        if (string.IsNullOrWhiteSpace(x.Name)) return 1;
        if (string.IsNullOrWhiteSpace(y.Name)) return -1;

        // Name compare
        return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
    }
}