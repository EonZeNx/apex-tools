using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

public class RtpcV03ValueOffsetMaps
{
    public readonly Dictionary<string, uint> StringOffsetMap = new();
    public readonly Dictionary<IList<float>, uint> Vec2OffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<float>, uint> Vec3OffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<float>, uint> Vec4OffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<float>, uint> Mat3OffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<float>, uint> Mat4OffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<uint>, uint> U32ArrayOffsetMap = new(new ListComparer<uint>());
    public readonly Dictionary<IList<float>, uint> F32ArrayOffsetMap = new(new ListComparer<float>());
    public readonly Dictionary<IList<byte>, uint> ByteArrayOffsetMap = new(new ListComparer<byte>());
    public readonly Dictionary<ulong, uint> ObjectIdOffsetMap = new();
    public readonly Dictionary<IList<(uint, uint)>, uint> EventOffsetMap = new(new ListComparer<(uint, uint)>());
    
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
        
        var values = Array.ConvertAll(rawValues, byte.Parse).ToList();
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
                .Select(value => value.Split("="))
                .Select(eventPairs => (uint.Parse(eventPairs[0]), uint.Parse(eventPairs[1])))
                .ToList();
            
            EventOffsetMap.TryAdd(eventTuples, 0);
        }
        
        nodes = xd.Descendants(EVariantType.ObjectId.GetXmlName()).ToArray();
        foreach (var node in nodes)
        {
            var value = ulong.Parse(node.Value);
            ObjectIdOffsetMap.TryAdd(value, 0);
        }
    }
}

public static class RtpcV03ValueOffsetMapsExtensions
{
    public static void Write(this BinaryWriter bw, RtpcV03ValueOffsetMaps voMaps)
    {
        foreach (var value in voMaps.StringOffsetMap.Keys)
        {
            var offset = (uint) bw.Position();
            voMaps.StringOffsetMap[value] = offset;
            
            bw.WriteStringZ(value);
        }
        
        foreach (var value in voMaps.Vec2OffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.Vec2OffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.Vec3OffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.Vec3OffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.Vec4OffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.Vec4OffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.Mat3OffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.Mat3OffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.Mat4OffsetMap.Keys)
        {
            bw.Align(16);
            
            var offset = (uint) bw.Position();
            voMaps.Mat4OffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.U32ArrayOffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.U32ArrayOffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.F32ArrayOffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.F32ArrayOffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.ByteArrayOffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.ByteArrayOffsetMap[value] = offset;
            
            bw.Write(value);
        }
        
        foreach (var value in voMaps.EventOffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.EventOffsetMap[value] = offset;
            
            bw.Write(value.Count);
            foreach (var valueTuple in value)
            {
                bw.Write(valueTuple.Item1);
                bw.Write(valueTuple.Item2);
            }
        }
        
        foreach (var value in voMaps.ObjectIdOffsetMap.Keys)
        {
            bw.Align(4);
            
            var offset = (uint) bw.Position();
            voMaps.ObjectIdOffsetMap[value] = offset;
            
            bw.Write(value);
        }
    }
}