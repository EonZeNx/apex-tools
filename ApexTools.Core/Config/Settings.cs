using System.Xml.Linq;
using System.Xml.Schema;

namespace ApexTools.Core.Config;

public static class Settings
{
    public static string XmlName => "Settings";
    private static string XmlFilePath { get; set; } = string.Empty;
    
    #region Variables

    public static Setting<bool> LogProgress { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(LogProgress))
        .SetDescription("Track progress of each file");
    
    public static Setting<bool> AutoClose { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(AutoClose))
        .SetDescription("Automatically close the tool after an action");
    
    public static Setting<string> DatabasePath { get; set; } = Setting<string>
        .Create(@"C:\Fake\Path\To\Database.db")
        .SetName(nameof(DatabasePath))
        .SetDescription("Absolute path to the database file");
    
    public static Setting<bool> PerformHashLookUp { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(PerformHashLookUp))
        .SetDescription("Try lookup hash values");
    
    public static Setting<bool> AlwaysOutputHash { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(AlwaysOutputHash))
        .SetDescription("Always output the hash even if the hash lookup was successful");
    
    public static Setting<bool> OutputValueOffset { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(OutputValueOffset))
        .SetDescription("Whether or not to output the offset of each value");
    
    public static Setting<bool> RtpcSortContainers { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcSortContainers))
        .SetDescription("Sort containers of Runtime Containers (I/RTPC files)");
    
    public static Setting<bool> RtpcSortProperties { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcSortProperties))
        .SetDescription("Sort properties of Runtime Containers (I/RTPC files)");
    
    public static Setting<bool> RtpcSkipUnassignedProperties { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcSkipUnassignedProperties))
        .SetDescription("Skip unassigned properties of Runtime Containers (I/RTPC files)");
    
    public static Setting<bool> RtpcUseJc4 { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcUseJc4))
        .SetDescription("Use JC4 RTPC v3(.1) instead of RTPC v3");

    #endregion

    public static void Load()
    {
        var exeDirectory = AppContext.BaseDirectory;
        XmlFilePath = Path.Combine(exeDirectory, "config.xml");
            
        if (!File.Exists(XmlFilePath))
        {
            WriteSettings();
        }

        try
        {
            LoadSettings();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            WriteSettings();
            LoadSettings();
        }
    }

    #region IO Functions
    
    public static void WriteSettings()
    {
        var xDocument = new XDocument();
        var xElement = new XElement(XmlName);

        WriteSetting(xElement, LogProgress);
        WriteSetting(xElement, AutoClose);
        WriteSetting(xElement, DatabasePath);
        WriteSetting(xElement, PerformHashLookUp);
        WriteSetting(xElement, AlwaysOutputHash);
        WriteSetting(xElement, OutputValueOffset);
        WriteSetting(xElement, RtpcSortProperties);
        WriteSetting(xElement, RtpcSkipUnassignedProperties);
        WriteSetting(xElement, RtpcUseJc4);
            
        xDocument.Add(xElement);
        xDocument.Save(XmlFilePath);
    }
    
    public static void LoadSettings()
    {
        var xDocument = XDocument.Load(XmlFilePath);
        
        var settingsXElement = xDocument.Element(XmlName);
        if (settingsXElement is null)
        {
            throw new XmlSchemaException($"{XmlName} does not exist in file");
        }

        LogProgress.Value = LoadSetting(settingsXElement, LogProgress);
        AutoClose.Value = LoadSetting(settingsXElement, AutoClose);
        DatabasePath.Value = LoadSetting(settingsXElement, DatabasePath);
        PerformHashLookUp.Value = LoadSetting(settingsXElement, PerformHashLookUp);
        AlwaysOutputHash.Value = LoadSetting(settingsXElement, AlwaysOutputHash);
        OutputValueOffset.Value = LoadSetting(settingsXElement, OutputValueOffset);
        RtpcSortProperties.Value = LoadSetting(settingsXElement, RtpcSortProperties);
        RtpcSkipUnassignedProperties.Value = LoadSetting(settingsXElement, RtpcSkipUnassignedProperties);
        RtpcUseJc4.Value = LoadSetting(settingsXElement, RtpcUseJc4);
    }
    
    private static void WriteSetting<T>(XContainer parentXContainer, Setting<T> setting)
    {
        var settingXElement = new XElement(Setting<T>.XmlName);
        
        var nameXElement = new XElement(nameof(setting.Name))
        {
            Value = setting.Name
        };
        
        var descriptionXElement = new XElement(nameof(setting.Description))
        {
            Value = setting.Description
        };

        var valueXElement = new XElement(nameof(setting.Value))
        {
            Value = $"{setting.Value}"
        };
        
        settingXElement.Add(nameXElement);
        settingXElement.Add(descriptionXElement);
        settingXElement.Add(valueXElement);
        
        parentXContainer.Add(settingXElement);
    }
    
    private static T LoadSetting<T>(this XContainer xContainer, Setting<T> setting)
    {
        var settingNode = xContainer.Element(Setting<T>.XmlName);
        if (settingNode is null)
        {
            return setting.Value;
        }

        var valueNode = settingNode.Element(nameof(setting.Value));
        if (valueNode is null)
        {
            return setting.Value;
        }
        
        var value = valueNode.Value;
        try
        {
            return (T) Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return setting.Value;
        }
    }

    #endregion
}

/// <summary>
/// Struct for generic settings, containing a value and a description
/// </summary>
/// <typeparam name="T"></typeparam>
public class Setting<T>
{
    public string Name;
    public T Value;
    public string Description;

    public static string XmlName => "Setting";
    
    public Setting(T value)
    {
        Name = string.Empty;
        Value = value;
        Description = string.Empty;
    }

    public static Setting<T> Create(T value)
    {
        return new Setting<T>(value);
    }

    public Setting<T> SetName(string name)
    {
        Name = name;

        return this;
    }

    public Setting<T> SetDescription(string description)
    {
        Description = description;

        return this;
    }
}