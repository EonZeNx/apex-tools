using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace EonZeNx.ApexFormats.RTPC.V03.Models;

public class PropertyV03Comparer : Comparer<PropertyBaseV03>
{
    public override int Compare(PropertyBaseV03? x, PropertyBaseV03? y)
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

public class ContainerV03Comparer : Comparer<ContainerV03>
{
    public override int Compare(ContainerV03? x, ContainerV03? y)
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