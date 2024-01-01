using System.Globalization;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Flat.Utils;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Flat.Models;

public class RtpcV03ValueOffsetMaps
{
    public readonly Dictionary<string, uint> StringOffsetMap = new();
    public readonly Dictionary<IList<float>, uint> Vec2OffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<float>, uint> Vec3OffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<float>, uint> Vec4OffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<float>, uint> Mat3OffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<float>, uint> Mat4OffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<uint>, uint> U32ArrayOffsetMap = new(new ListEqualityComparer<uint>());
    public readonly Dictionary<IList<float>, uint> F32ArrayOffsetMap = new(new ListEqualityComparer<float>());
    public readonly Dictionary<IList<byte>, uint> ByteArrayOffsetMap = new(new ListEqualityComparer<byte>());
    public readonly Dictionary<ulong, uint> ObjectIdOffsetMap = new();
    public readonly Dictionary<IList<(uint, uint)>, uint> EventOffsetMap = new(new ListEqualityComparer<(uint, uint)>());
    
    protected static HashSet<IList<(uint, uint)>> ReadEvents(XmlNodeList nodes, uint count = 0)
    {
        var result = new HashSet<IList<(uint, uint)>>();
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            if (node is null)
            {
                throw new XmlSchemaException("Invalid Events node");
            }

            var rawValue = node.Value;
            if (rawValue is null)
            {
                throw new XmlSchemaException("Events node has no value");
            }
            
            var rawValues = rawValue.Split(",");
            if (count != 0 && rawValues.Length != count)
            {
                throw new XmlSchemaException($"Events node does not contain {count} values");
            }

            var eventTuples = rawValues
                .Select(value => value.Split("="))
                .Select(eventPairs => (uint.Parse(eventPairs[0]), uint.Parse(eventPairs[1])))
                .ToList();

            result.Add(eventTuples);
        }

        return result;
    }
    
    protected static IList<float> ParseF32Array(string strValue, int count = 0)
    {
        var rawValues = strValue.Split(",");
        if (count != 0 && rawValues.Length != count)
        {
            throw new XmlSchemaException($"F32Array node does not contain {count} values");
        }
        
        var values = Array.ConvertAll(rawValues, float.Parse).ToList();
        return values;
    }
    
    protected static IList<byte> ParseByteArray(string strValue, int count = 0)
    {
        var rawValues = strValue.Split(",");
        if (count != 0 && rawValues.Length != count)
        {
            throw new XmlSchemaException($"ByteArray node does not contain {count} values");
        }
        
        var values = rawValues
            .Select(s => byte.Parse(s, NumberStyles.HexNumber))
            .ToArray();
        return values;
    }
    
    protected static IList<uint> ParseU32Array(string strValue, int count = 0)
    {
        var rawValues = strValue.Split(",");
        if (count != 0 && rawValues.Length != count)
        {
            throw new XmlSchemaException($"F32Array node does not contain {count} values");
        }
        
        var values = Array.ConvertAll(rawValues, uint.Parse).ToList();
        return values;
    }

    public void Create(XDocument xd)
    {
        var nodes = xd.Descendants(EVariantType.String.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            StringOffsetMap.TryAdd(node.Value, 0);
        }

        nodes = xd.Descendants(EVariantType.Vector2.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value, 2);
            Vec2OffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Vector3.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value, 3);
            Vec3OffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Vector4.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value, 4);
            Vec4OffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Matrix3X3.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value, 9);
            Mat3OffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Matrix4X4.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value, 16);
            Mat4OffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.UInteger32Array.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseU32Array(node.Value);
            U32ArrayOffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Float32Array.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseF32Array(node.Value);
            F32ArrayOffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.ByteArray.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var values = ParseByteArray(node.Value);
            ByteArrayOffsetMap.TryAdd(values, 0);
        }
        
        nodes = xd.Descendants(EVariantType.Event.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var rawValues = node.Value.Split(",");
            var eventTuples = rawValues
                .Select(value => value.Split("=")
                    .Select(v => uint.Parse(v, NumberStyles.HexNumber)).ToArray())
                .Select(eventPairs => (eventPairs[0], eventPairs[1]))
                .ToList();
            
            EventOffsetMap.TryAdd(eventTuples, 0);
        }
        
        nodes = xd.Descendants(EVariantType.ObjectId.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var value = ulong.Parse(node.Value, NumberStyles.HexNumber);
            ObjectIdOffsetMap.TryAdd(value, 0);
        }
        
        // // Include container attributes
        // nodes = xd.Descendants(RtpcV03Container.XmlName).ToArray();
        // foreach (var node in nodes)
        // {
        //     // Name (optional)
        //     var attribute = node.Attribute(XElementExtensions.NameAttributeName);
        //     if (attribute is not null)
        //     {
        //         StringOffsetMap.TryAdd(attribute.Value, 0);
        //     }
        //
        //     // Object ID
        //     var oIdXmlName = EVariantType.ObjectId.GetXmlName();
        //
        //     attribute = node.Attribute(oIdXmlName);
        //     if (attribute is not null)
        //     {
        //         var value = ulong.Parse(attribute.Value, NumberStyles.HexNumber);
        //         ObjectIdOffsetMap.TryAdd(value, 0);
        //         
        //         continue;
        //     }
        //
        //     // Root container parent is not RtpcV03Container.XmlName
        //     if (node.Parent is not null && node.Parent.Name == RtpcV03Container.XmlName)
        //     {
        //         throw new XmlSchemaException($"{RtpcV03Container.XmlName} does not have {oIdXmlName}");
        //     }
        // }
    }
}

public static class RtpcV03ValueOffsetMapsExtensions
{
    public static IEnumerable<string> SortStringKeys(this RtpcV03ValueOffsetMaps _, IEnumerable<string> strKeys)
    {
        var sortedKeys = strKeys.ToList();
        sortedKeys.Sort(StringComparer.Ordinal);

        return sortedKeys;
    }
    
    public static IEnumerable<IList<T>> SortNumericArrayKeys<T>(this RtpcV03ValueOffsetMaps _, IEnumerable<IList<T>> arrayKeys) where T : INumber<T>
    {
        var sortedKeys = arrayKeys.OrderBy(k => k, new ListComparer<T>());

        return sortedKeys;
    }
    
    public static IEnumerable<IList<float>> SortF32ArrayKeys(this RtpcV03ValueOffsetMaps _, IEnumerable<IList<float>> f32ArrayKeys)
    {
        var sortedKeys = f32ArrayKeys.OrderBy(k => k, new ListComparer<float>());

        return sortedKeys;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03ValueOffsetMaps voMaps)
    {
        // ReSharper disable JoinDeclarationAndInitializer
        IEnumerable<string> sortedStrKeys;
        IEnumerable<IList<float>> sortedF32ArrayKeys;
        // ReSharper enable JoinDeclarationAndInitializer
        
        sortedStrKeys = voMaps.SortStringKeys(voMaps.StringOffsetMap.Keys);
        foreach (var key in sortedStrKeys)
        {
            bw.Align(EVariantType.String.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.StringOffsetMap[key] = offset;
            
            bw.WriteStringZ(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.Vec2OffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Vector2.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.Vec2OffsetMap[key] = offset;
            
            bw.Write(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.Vec3OffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Vector3.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.Vec3OffsetMap[key] = offset;
            
            bw.Write(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.Vec4OffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Vector4.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.Vec4OffsetMap[key] = offset;
            
            bw.Write(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.Mat3OffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Matrix3X3.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.Mat3OffsetMap[key] = offset;
            
            bw.Write(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.Mat4OffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Matrix4X4.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.Mat4OffsetMap[key] = offset;
            
            bw.Write(key);
        }
        ;
        var sortedU32ArrayKeys = voMaps.SortNumericArrayKeys(voMaps.U32ArrayOffsetMap.Keys);
        foreach (var key in sortedU32ArrayKeys)
        {
            bw.Align(EVariantType.UInteger32Array.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.U32ArrayOffsetMap[key] = offset;
            
            bw.Write((uint) key.Count);
            bw.Write(key);
        }
        ;
        sortedF32ArrayKeys = voMaps.SortF32ArrayKeys(voMaps.F32ArrayOffsetMap.Keys);
        foreach (var key in sortedF32ArrayKeys)
        {
            bw.Align(EVariantType.Float32Array.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.F32ArrayOffsetMap[key] = offset;
            
            bw.Write((uint) key.Count);
            bw.Write(key);
        }
        ;
        var sortedByteArrayKeys = voMaps.SortNumericArrayKeys(voMaps.ByteArrayOffsetMap.Keys);
        foreach (var value in sortedByteArrayKeys)
        {
            bw.Align(EVariantType.ByteArray.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.ByteArrayOffsetMap[value] = offset;
            
            bw.Write((uint) value.Count);
            bw.Write(value);
        }
        ;
        foreach (var value in voMaps.EventOffsetMap.Keys)
        {
            bw.Align(EVariantType.Event.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.EventOffsetMap[value] = offset;
            
            bw.Write((uint) value.Count);
            foreach (var valueTuple in value)
            {
                bw.Write(valueTuple.Item1);
                bw.Write(valueTuple.Item2);
            }
        }
        ;
        var sortedOIdArrayKeys = voMaps.ObjectIdOffsetMap.Keys.ToList().OrderBy(k => k);
        foreach (var value in sortedOIdArrayKeys)
        {
            bw.Align(EVariantType.ObjectId.GetAlignment());
            
            var offset = (uint) bw.Position();
            voMaps.ObjectIdOffsetMap[value] = offset;
            
            bw.Write(value);
        }
    }
}