using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Extensions;

namespace ApexFormat.RTPC.V03.Flat.Models;

public class RtpcV03OffsetValueMaps
{
    public readonly Dictionary<uint, string> OffsetStringMap = new();
    public readonly Dictionary<uint, IList<float>> OffsetVec2Map = new();
    public readonly Dictionary<uint, IList<float>> OffsetVec3Map = new();
    public readonly Dictionary<uint, IList<float>> OffsetVec4Map = new();
    public readonly Dictionary<uint, IList<float>> OffsetMat3X3Map = new();
    public readonly Dictionary<uint, IList<float>> OffsetMat4X4Map = new();
    public readonly Dictionary<uint, IList<uint>> OffsetU32ArrayMap = new();
    public readonly Dictionary<uint, IList<float>> OffsetF32ArrayMap = new();
    public readonly Dictionary<uint, IList<byte>> OffsetByteArrayMap = new();
    public readonly Dictionary<uint, ulong> OffsetObjectIdMap = new();
    public readonly Dictionary<uint, IList<(uint, uint)>> OffsetEventMap = new();


    public string GetAsString(byte[] data, EVariantType variant, bool tryHex = false)
    {
        var dataU32 = BitConverter.ToUInt32(data);
        var hexU32 = $"{dataU32:X8}";
        var u32 = tryHex ? hexU32 : $"{dataU32}";
        
        return variant switch
        {
            EVariantType.Unassigned => string.Empty,
            EVariantType.UInteger32 => u32,
            EVariantType.Float32 => $"{BitConverter.ToSingle(data)}",
            EVariantType.String => $"{OffsetStringMap[dataU32]}",
            EVariantType.Vector2 => $"{OffsetVec2Map[dataU32]}",
            EVariantType.Vector3 => $"{OffsetVec3Map[dataU32]}",
            EVariantType.Vector4 => $"{OffsetVec4Map[dataU32]}",
            EVariantType.Matrix3X3 => $"{OffsetMat3X3Map[dataU32]}",
            EVariantType.Matrix4X4 => $"{OffsetMat4X4Map[dataU32]}",
            EVariantType.UInteger32Array => $"{OffsetU32ArrayMap[dataU32]}",
            EVariantType.Float32Array => $"{OffsetF32ArrayMap[dataU32]}",
            EVariantType.ByteArray => $"{OffsetByteArrayMap[dataU32]}",
            EVariantType.Deprecated => string.Empty,
            EVariantType.ObjectId => $"{OffsetObjectIdMap[dataU32]:X16}",
            EVariantType.Events => $"{OffsetEventMap[dataU32]}",
            EVariantType.Total => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, null)
        };
    }
    
    protected static IList<byte> ReadByteArray(BinaryReader br)
    {
        var result = new List<byte>();
        var count = br.ReadUInt32();
        
        for (var i = 0; i < count; i++)
        {
            result.Add(br.ReadByte());
        }

        return result;
    }
    
    protected void CreateU64Array(BinaryReader br)
    {
        // Count will be in bytes
        var byteCount = br.ReadUInt32();
        var oidCount = byteCount / 8;

        for (var i = 0; i < oidCount; i++)
        {
            var offset = (uint) br.Position();
            var oId = br.ReadUInt64();
                    
            OffsetObjectIdMap.Add(offset, oId);
        }
    }
    
    protected static IList<float> ReadF32Array(BinaryReader br)
    {
        var count = br.ReadUInt32();
        return ReadFixedF32Array(br, count);
    }
    
    protected static IList<uint> ReadU32Array(BinaryReader br)
    {
        var result = new List<uint>();
        var count = br.ReadUInt32();
        
        for (var i = 0; i < count; i++)
        {
            result.Add(br.ReadUInt32());
        }

        return result;
    }
    
    protected static IList<float> ReadFixedF32Array(BinaryReader br, uint count = 0)
    {
        // if (count == 0)
        // {
        //     count = br.ReadUInt32();
        // }
        
        var result = new List<float>();
        for (var i = 0; i < count; i++)
        {
            result.Add(br.ReadSingle());
        }

        return result;
    }

    protected static IList<(uint, uint)> ReadEventArray(BinaryReader br)
    {
        var count = br.ReadUInt32();
        var eventArray = new List<(uint, uint)>((int)count);

        for (var j = 0; j < count; j++)
        {
            var first = br.ReadUInt32();
            var second = br.ReadUInt32();
            eventArray.Add((first, second));
        }

        return eventArray;
    }

    public void Create(BinaryReader br, IEnumerable<RtpcV03PropertyHeader> uniqueOffsets)
    {
        var orderedOffsets = uniqueOffsets
            .OrderBy(header => header.DataAsOffset())
            .ToArray();

        foreach (var header in orderedOffsets)
        {
            var offset = header.DataAsOffset();

            if (br.Position() != offset)
            {
                br.Seek(offset);
            }
            
            switch (header.VariantType)
            {
                case EVariantType.String:
                    OffsetStringMap.Add(offset, br.ReadStringZ());
                    break;
                case EVariantType.Vector2:
                    OffsetVec2Map.Add(offset, ReadFixedF32Array(br, 2));
                    break;
                case EVariantType.Vector3:
                    OffsetVec3Map.Add(offset, ReadFixedF32Array(br, 3));
                    break;
                case EVariantType.Vector4:
                    OffsetVec4Map.Add(offset, ReadFixedF32Array(br, 4));
                    break;
                case EVariantType.Matrix3X3:
                    OffsetMat3X3Map.Add(offset, ReadFixedF32Array(br, 9));
                    break;
                case EVariantType.Matrix4X4:
                    OffsetMat4X4Map.Add(offset, ReadFixedF32Array(br, 16));
                    break;
                case EVariantType.UInteger32Array:
                    OffsetU32ArrayMap.Add(offset, ReadU32Array(br));
                    break;
                case EVariantType.Float32Array:
                    OffsetF32ArrayMap.Add(offset, ReadF32Array(br));
                    break;
                case EVariantType.ByteArray:
                    OffsetByteArrayMap.Add(offset, ReadByteArray(br));
                    break;
                case EVariantType.ObjectId:
                    OffsetObjectIdMap.Add(offset, br.ReadUInt64());
                    break;
                case EVariantType.Events:
                    OffsetEventMap.Add(offset, ReadEventArray(br));
                    break;
                case EVariantType.Unassigned:
                case EVariantType.UInteger32:
                case EVariantType.Float32:
                case EVariantType.Deprecated:
                case EVariantType.Total:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}