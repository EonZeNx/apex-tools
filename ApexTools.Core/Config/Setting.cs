using System.Xml.Linq;
using System.Xml.Schema;

namespace ApexTools.Core.Config;

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

public static class SettingExtensions
{
    public static void WriteSetting<T>(this XContainer parentXContainer, Setting<T> setting)
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
    
    public static T LoadSetting<T>(this XContainer xContainer, Setting<T> setting)
    {
        var settingNode = xContainer.Element(Setting<T>.XmlName);
        var settingNameNode = settingNode?.Element(nameof(setting.Name));
        
        while (settingNode is not null)
        {
            if (settingNameNode?.Value == setting.Name) break;
            if (settingNode.NextNode is null) break;
            
            settingNode = (XElement) settingNode.NextNode;
            settingNameNode = settingNode?.Element(nameof(setting.Name));
        }

        if (settingNode is null || settingNameNode?.Value != setting.Name)
        {
            throw new XmlSchemaException($"Could not load {setting.Name}, missing setting node");
        }

        var valueNode = settingNode.Element(nameof(setting.Value));
        if (valueNode is null)
        {
            throw new XmlSchemaException($"Could not load {setting.Name}, missing value node");
        }
        
        var value = valueNode.Value;
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