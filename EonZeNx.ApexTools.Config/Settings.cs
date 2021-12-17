namespace EonZeNx.ApexTools.Config;

public static class Settings
{
    public static Setting<bool> AutoClose { get; set; } = new()
    {
        Value = false,
        Description = "Automatically close the tool after an action",
        DefaultValue = false
    };
    
    public static Setting<string> DatabasePath { get; set; } = new()
    {
        Value = @"E:\Projects\Just Cause Tools\Apex.Tools.Refresh\EonZeNx.ApexTools.Core\db\ApexToolsMain.db",
        Description = "Absolute path to the database file",
        DefaultValue = @"C:\Fake\Path\To\Database.db"
    };
    
    public static Setting<bool> PerformHashLookUp { get; set; } = new()
    {
        Value = true,
        Description = "Try lookup the hash for values where possible",
        DefaultValue = true
    };
    
    public static Setting<ushort> HashCacheSize { get; set; } = new()
    {
        Value = 250,
        Description = "The maximum amount of hashes to cache",
        DefaultValue = 250
    };
    
    public static Setting<bool> AlwaysOutputHash { get; set; } = new()
    {
        Value = true,
        Description = "Always output the hash even if the hash lookup was successful",
        DefaultValue = true
    };
    
    public static Setting<bool> SortRuntimeContainers { get; set; } = new()
    {
        Value = false,
        Description = "Whether or not to sort any Runtime Containers (I/RTPC files)",
        DefaultValue = true
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