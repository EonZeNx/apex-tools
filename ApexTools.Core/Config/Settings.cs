using System.Xml.Linq;
using System.Xml.Schema;

namespace ApexTools.Core.Config;

public static class Settings
{
    private static string Resources { get; set; } = Path.Join(AppContext.BaseDirectory, "resources");
    private static string FileName { get; set; } = "settings.xml";
    private static string FilePath { get; set; } = string.Empty;
    
    #region Variables

    public static Setting<bool> AutoClose { get; set; } = Setting<bool>
        .Create(false)
        .SetName(nameof(AutoClose));

    public static Setting<string> DatabasePath { get; set; } = Setting<string>
        .Create(Path.Join(Resources, "databases", "ApexTools.Core.db"))
        .SetName(nameof(DatabasePath));

    public static Setting<bool> LookupHashes { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(LookupHashes));

    public static Setting<bool> PreloadHashes { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(PreloadHashes));

    public static Setting<bool> OutputNameHash { get; set; } = Setting<bool>
        .Create(false)
        .SetName(nameof(OutputNameHash));

    public static Setting<bool> RtpcSortProperties { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcSortProperties));
    
    public static Setting<bool> RtpcSkipUnassignedProperties { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcSkipUnassignedProperties))
        .SetDescription("Skip unassigned properties of Runtime Containers (I/RTPC files)");

    public static Setting<bool> RtpcPreferFlat { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcPreferFlat));

    public static Setting<string> RtpcClassDefinitionDirectory { get; set; } = Setting<string>
        .Create(Path.Join(Resources, "rtpc_class_definitions"))
        .SetName(nameof(RtpcClassDefinitionDirectory));

    public static Setting<bool> RtpcUpdateClassDefinitions { get; set; } = Setting<bool>
        .Create(true)
        .SetName(nameof(RtpcUpdateClassDefinitions));

    #endregion

    public static void Load()
    {
        FilePath = Path.Combine(Resources, FileName);
            
        if (!File.Exists(FilePath))
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
        var xElement = new XElement(nameof(Settings));

        xElement.WriteSetting(AutoClose);
        xElement.WriteSetting(DatabasePath);
        xElement.WriteSetting(LookupHashes);
        xElement.WriteSetting(PreloadHashes);
        xElement.WriteSetting(OutputNameHash);
        xElement.WriteSetting(RtpcSortProperties);
        xElement.WriteSetting(RtpcSkipUnassignedProperties);
        xElement.WriteSetting(RtpcPreferFlat);
        xElement.WriteSetting(RtpcClassDefinitionDirectory);
        xElement.WriteSetting(RtpcUpdateClassDefinitions);
            
        xDocument.Add(xElement);
        xDocument.Save(FilePath);
    }
    
    public static void LoadSettings()
    {
        var xDocument = XDocument.Load(FilePath);
        
        var settingsXElement = xDocument.Element(nameof(Settings));
        if (settingsXElement is null)
        {
            throw new XmlSchemaException($"{nameof(Settings)} does not exist in file");
        }

        AutoClose.Value = settingsXElement.LoadSetting(AutoClose);
        DatabasePath.Value = settingsXElement.LoadSetting(DatabasePath);
        LookupHashes.Value = settingsXElement.LoadSetting(LookupHashes);
        PreloadHashes.Value = settingsXElement.LoadSetting(PreloadHashes);
        OutputNameHash.Value = settingsXElement.LoadSetting(OutputNameHash);
        RtpcSortProperties.Value = settingsXElement.LoadSetting(RtpcSortProperties);
        RtpcSkipUnassignedProperties.Value = settingsXElement.LoadSetting(RtpcSkipUnassignedProperties);
        RtpcPreferFlat.Value = settingsXElement.LoadSetting(RtpcPreferFlat);
        RtpcClassDefinitionDirectory.Value = settingsXElement.LoadSetting(RtpcClassDefinitionDirectory);
        RtpcUpdateClassDefinitions.Value = settingsXElement.LoadSetting(RtpcUpdateClassDefinitions);
    }

    #endregion
}
