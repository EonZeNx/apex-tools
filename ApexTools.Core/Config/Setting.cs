using System.Xml.Linq;
using System.Xml.Schema;

namespace ApexTools.Core.Config;

/// <summary>
/// Struct for generic settings, containing a value and a description
/// </summary>
/// <typeparam name="T"></typeparam>
public class Setting<T> where T : notnull
{
    public string Name;
    public string XmlName;
    public T Value;
    
    public Setting(T value)
    {
        Name = string.Empty;
        XmlName = string.Empty;
        Value = value;
    }

    public static Setting<T> Create(T value)
    {
        return new Setting<T>(value);
    }

    public Setting<T> SetName(string name, bool updateXmlName = true)
    {
        Name = name;
        if (updateXmlName)
        {
            XmlName = name;
        }

        return this;
    }

    public Setting<T> SetXmlName(string xmlName)
    {
        XmlName = xmlName;

        return this;
    }
}

public static class SettingExtensions
{
    public static void WriteSetting<T>(this XContainer parentXContainer, Setting<T> setting) where T : notnull
    {
        var settingXElement = new XElement(setting.XmlName);

        settingXElement.SetValue(setting.Value);
        
        parentXContainer.Add(settingXElement);
    }
    
    public static T LoadSetting<T>(this XContainer xContainer, Setting<T> setting) where T : notnull
    {
        var settingNode = xContainer.Element(setting.XmlName);
        if (settingNode is null)
        {
            throw new XmlSchemaException($"Missing {setting.XmlName} setting");
        }
        
        var value = settingNode.Value;
        try
        {
            return (T) Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            throw new XmlSchemaException($"Could not load {setting.Name}, invalid value");
        }
    }
}