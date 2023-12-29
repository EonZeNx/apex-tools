using System.Xml.Linq;
using System.Xml.Schema;

namespace ApexTools.Core.Config;

public static class Settings
{
    public static string XmlName => "Settings";
    private static string XmlFileName { get; set; } = "config.xml";
    private static string XmlFilePath { get; set; } = string.Empty;
    
    #region Variables

    public static Setting<bool> LogProgress { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(LogProgress))
        .SetDescription("Track progress of each file");
    
    public static Setting<bool> AutoClose { get; set; } = Setting<bool>
        .Create(false)
        .SetName(nameof(AutoClose))
        .SetDescription("Automatically close the tool after an action");
    
    public static Setting<string> DatabasePath { get; set; } = Setting<string>
        .Create(Path.Join(AppContext.BaseDirectory, "resources", "databases", "ApexTools.Core.db"))
        .SetName(nameof(DatabasePath))
        .SetDescription("Absolute path to the database file");
    
    public static Setting<bool> PerformHashLookUp { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(PerformHashLookUp))
        .SetDescription("Try lookup hash values");
    
    public static Setting<bool> LoadAllHashes { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(LoadAllHashes))
        .SetDescription("Pre-load all hashes on startup");
    
    public static Setting<bool> AlwaysOutputHash { get; set; } = Setting<bool>
        .Create(false)
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
    
    public static Setting<bool> RtpcPreferFlat { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcPreferFlat))
        .SetDescription("Prefer flat RTPC v3");
    
    public static Setting<string> RtpcClassDirectory { get; set; } = Setting<string>
        .Create(Path.Join(AppContext.BaseDirectory, "resources", "rtpc_class_definitions"))
        .SetName(nameof(RtpcClassDirectory))
        .SetDescription("Location of flat RTPC v03 class definitions");
    
    public static Setting<bool> RtpcUpdateClassDefinitions { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcUpdateClassDefinitions))
        .SetDescription("Always write RTPC class definitions");

    #endregion

    public static void Load()
    {
        var exeDirectory = AppContext.BaseDirectory;
        XmlFilePath = Path.Combine(exeDirectory, XmlFileName);
            
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

        xElement.WriteSetting(LogProgress);
        xElement.WriteSetting(AutoClose);
        xElement.WriteSetting(DatabasePath);
        xElement.WriteSetting(PerformHashLookUp);
        xElement.WriteSetting(LoadAllHashes);
        xElement.WriteSetting(AlwaysOutputHash);
        xElement.WriteSetting(OutputValueOffset);
        xElement.WriteSetting(RtpcSortProperties);
        xElement.WriteSetting(RtpcSkipUnassignedProperties);
        xElement.WriteSetting(RtpcPreferFlat);
        xElement.WriteSetting(RtpcClassDirectory);
        xElement.WriteSetting(RtpcUpdateClassDefinitions);
            
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

        LogProgress.Value = settingsXElement.LoadSetting(LogProgress);
        AutoClose.Value = settingsXElement.LoadSetting(AutoClose);
        DatabasePath.Value = settingsXElement.LoadSetting(DatabasePath);
        PerformHashLookUp.Value = settingsXElement.LoadSetting(PerformHashLookUp);
        LoadAllHashes.Value = settingsXElement.LoadSetting(LoadAllHashes);
        AlwaysOutputHash.Value = settingsXElement.LoadSetting(AlwaysOutputHash);
        OutputValueOffset.Value = settingsXElement.LoadSetting(OutputValueOffset);
        RtpcSortProperties.Value = settingsXElement.LoadSetting(RtpcSortProperties);
        RtpcSkipUnassignedProperties.Value = settingsXElement.LoadSetting(RtpcSkipUnassignedProperties);
        RtpcPreferFlat.Value = settingsXElement.LoadSetting(RtpcPreferFlat);
        RtpcClassDirectory.Value = settingsXElement.LoadSetting(RtpcClassDirectory);
        RtpcUpdateClassDefinitions.Value = settingsXElement.LoadSetting(RtpcUpdateClassDefinitions);
    }

    #endregion
}
