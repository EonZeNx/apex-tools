using EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

namespace EonZeNx.ApexFormats.RTPC.V01.Models;

public class PropertyV01Comparer : Comparer<PropertyBaseV01>
{
    public override int Compare(PropertyBaseV01? x, PropertyBaseV01? y)
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

public class ContainerV01Comparer : Comparer<ContainerV01>
{
    public override int Compare(ContainerV01? x, ContainerV01? y)
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