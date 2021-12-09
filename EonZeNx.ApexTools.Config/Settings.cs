namespace EonZeNx.ApexTools.Config;

public static class Settings
{
    public static Setting<bool> AutoClose { get; set; } = new()
    {
        Value = false,
        Description = "Automatically close the tool after an action",
        DefaultValue = false
    };
}

/// <summary>
/// Struct for generic settings, containing a value, description, and default value
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Setting<T>
{
    public T Value { get; set; }
    public string Description { get; set; }
    public T DefaultValue { get; set; }
}