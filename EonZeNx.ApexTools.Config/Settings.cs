using System.Xml;

namespace EonZeNx.ApexTools.Config;

public static class Settings
{
    #region Variables

    private static string ExeFilepath { get; set; } = string.Empty;
    
    public static Setting<bool> LogProgress { get; set; } = new()
    {
        Name = nameof(LogProgress),
        Value = true,
        Description = "Track progress of each file",
        DefaultValue = true
    };
    
    public static Setting<bool> AutoClose { get; set; } = new()
    {
        Name = nameof(AutoClose),
        Value = true,
        Description = "Automatically close the tool after an action",
        DefaultValue = false
    };
    
    public static Setting<string> DatabasePath { get; set; } = new()
    {
        Name = nameof(DatabasePath),
        Value = @"E:\Projects\Just Cause Tools\Apex.Tools.Refresh\EonZeNx.ApexTools.Core\db\ApexToolsMain.db",
        Description = "Absolute path to the database file",
        DefaultValue = @"C:\Fake\Path\To\Database.db"
    };
    
    public static Setting<bool> PerformHashLookUp { get; set; } = new()
    {
        Name = nameof(PerformHashLookUp),
        Value = true,
        Description = "Try lookup the hash for values where possible",
        DefaultValue = true
    };
    
    public static Setting<ushort> HashCacheSize { get; set; } = new()
    {
        Name = nameof(HashCacheSize),
        Value = 250,
        Description = "The maximum amount of hashes to cache",
        DefaultValue = 250
    };
    
    public static Setting<bool> AlwaysOutputHash { get; set; } = new()
    {
        Name = nameof(AlwaysOutputHash),
        Value = true,
        Description = "Always output the hash even if the hash lookup was successful",
        DefaultValue = true
    };
    
    public static Setting<bool> OutputValueOffset { get; set; } = new()
    {
        Name = nameof(OutputValueOffset),
        Value = true,
        Description = "Whether or not to output the offset of each value",
        DefaultValue = true
    };
    
    public static Setting<bool> SortRuntimeContainers { get; set; } = new()
    {
        Name = nameof(SortRuntimeContainers),
        Value = false,
        Description = "Whether or not to sort any Runtime Containers (I/RTPC files)",
        DefaultValue = true
    };

    #endregion

    public static void LoadSettings()
    {
        var exeDirectory = AppContext.BaseDirectory;
        ExeFilepath = Path.Combine(exeDirectory, "config.xml");
            
        if (!File.Exists(ExeFilepath)) DumpDefaultSettings();

        LoadWrittenSettings();
    }

    #region IO Functions
    
    public static void DumpDefaultSettings()
    {
        var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
        var xw = XmlWriter.Create(ExeFilepath, settings);
        
        xw.WriteStartElement("Settings");

        WriteSetting(xw, LogProgress);
        WriteSetting(xw, AutoClose);
        WriteSetting(xw, DatabasePath);
        WriteSetting(xw, PerformHashLookUp);
        WriteSetting(xw, HashCacheSize);
        WriteSetting(xw, AlwaysOutputHash);
        WriteSetting(xw, OutputValueOffset);
        WriteSetting(xw, SortRuntimeContainers);
            
        xw.WriteEndElement();
        xw.Close();
    }
    
    public static void LoadWrittenSettings()
    {
        var xr = XmlReader.Create(ExeFilepath);
        xr.ReadToDescendant("Settings");

        LogProgress.Value = LoadSetting<bool>(xr, nameof(LogProgress));
        AutoClose.Value = LoadSetting<bool>(xr, nameof(AutoClose));
        DatabasePath.Value = LoadSetting<string>(xr, nameof(DatabasePath));
        PerformHashLookUp.Value = LoadSetting<bool>(xr, nameof(PerformHashLookUp));
        HashCacheSize.Value = LoadSetting<ushort>(xr, nameof(HashCacheSize));
        AlwaysOutputHash.Value = LoadSetting<bool>(xr, nameof(AlwaysOutputHash));
        OutputValueOffset.Value = LoadSetting<bool>(xr, nameof(OutputValueOffset));
        SortRuntimeContainers.Value = LoadSetting<bool>(xr, nameof(SortRuntimeContainers));
    }

    private static void WriteSetting<T>(XmlWriter xw, Setting<T> setting)
    {
        xw.WriteStartElement(setting.Name);
            
        xw.WriteStartElement(nameof(setting.Value));
        xw.WriteAttributeString(nameof(setting.DefaultValue), $"{setting.DefaultValue}");
        xw.WriteValue($"{setting.DefaultValue}");
        xw.WriteEndElement();
            
        xw.WriteStartElement(nameof(setting.Description));
        xw.WriteValue(setting.Description);
        xw.WriteEndElement();
            
        xw.WriteEndElement();
    }

    private static T LoadSetting<T>(XmlReader xr, string settingName)
    {
        xr.ReadToFollowing(settingName);
        xr.ReadToFollowing(nameof(Setting<T>.Value));
        
        var value = xr.ReadElementContentAsString();
        return (T) Convert.ChangeType(value, typeof(T));
    }

    #endregion
}

/// <summary>
/// Struct for generic settings, containing a value, description, and default value
/// </summary>
/// <typeparam name="T"></typeparam>
public class Setting<T>
{
    public string Name { get; set; } = string.Empty;
    public T? Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public T? DefaultValue { get; set; }
}