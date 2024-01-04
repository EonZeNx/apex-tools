using ApexFormat.RTPC.V03.Flat.Models.Data;
using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Utils;

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
            EVariantType.Event => $"{OffsetEventMap[dataU32]}",
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

    public void Create(BinaryReader br, RtpcV03PropertyHeader[] uniqueOffsets)
    {
        {
            var uniqueStringsCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.String);
            for (var i = 0; i < uniqueStringsCount; i++)
            {
                var offset = (uint) br.Position();
                var stringZ = br.ReadStringZ();
                OffsetStringMap.Add(offset, stringZ);
            }
        }
        ;
        {
            var uniqueVec2Count = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Vector2);
            for (var i = 0; i < uniqueVec2Count; i++)
            {
                br.Align(EVariantType.Vector2.GetAlignment());

                var offset = (uint)br.Position();
                var f32Array = ReadFixedF32Array(br, 2);

                OffsetVec2Map.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueVec3Count = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Vector3);
            for (var i = 0; i < uniqueVec3Count; i++)
            {
                br.Align(EVariantType.Vector3.GetAlignment());

                var offset = (uint)br.Position();
                var f32Array = ReadFixedF32Array(br, 3);

                OffsetVec3Map.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueVec4Count = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Vector4);
            for (var i = 0; i < uniqueVec4Count; i++)
            {
                br.Align(EVariantType.Vector4.GetAlignment());

                var offset = (uint) br.Position();
                var f32Array = ReadFixedF32Array(br, 4);

                OffsetVec4Map.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueMat3Count = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Matrix3X3);
            for (var i = 0; i < uniqueMat3Count; i++)
            {
                br.Align(EVariantType.Matrix3X3.GetAlignment());

                var offset = (uint)br.Position();
                var f32Array = ReadFixedF32Array(br, 9);

                OffsetMat3X3Map.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueMat4Count = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Matrix4X4);
            for (var i = 0; i < uniqueMat4Count; i++)
            {
                br.Align(EVariantType.Matrix4X4.GetAlignment());

                var offset = (uint)br.Position();
                var f32Array = ReadFixedF32Array(br, 16);

                OffsetMat4X4Map.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueU32ArrayCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.UInteger32Array);
            for (var i = 0; i < uniqueU32ArrayCount; i++)
            {
                br.Align(EVariantType.UInteger32Array.GetAlignment());

                var offset = (uint)br.Position();
                var u32Array = ReadU32Array(br);

                OffsetU32ArrayMap.Add(offset, u32Array);
            }
        }
        ;
        {
            var uniqueF32ArrayCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Float32Array);
            for (var i = 0; i < uniqueF32ArrayCount; i++)
            {
                br.Align(EVariantType.Float32Array.GetAlignment());

                var offset = (uint)br.Position();
                var f32Array = ReadF32Array(br);

                OffsetF32ArrayMap.Add(offset, f32Array);
            }
        }
        ;
        {
            var uniqueByteArrayCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.ByteArray);
            for (var i = 0; i < uniqueByteArrayCount; i++)
            {
                br.Align(EVariantType.ByteArray.GetAlignment());

                var offset = (uint) br.Position();
                var byteArray = ReadByteArray(br);

                OffsetByteArrayMap.Add(offset, byteArray);
            }
        }
        ;
        {
            var uniqueEventArrayCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.Event);
            for (var i = 0; i < uniqueEventArrayCount; i++)
            {
                br.Align(EVariantType.Event.GetAlignment());

                var offset = (uint)br.Position();

                var count = br.ReadUInt32();
                var eventArray = new List<(uint, uint)>((int)count);

                for (var j = 0; j < count; j++)
                {
                    var first = br.ReadUInt32();
                    var second = br.ReadUInt32();
                    eventArray.Add((first, second));
                }

                OffsetEventMap.Add(offset, eventArray);
            }
        }
        ;
        {
            var uniqueOIdCount = uniqueOffsets.Count(ph => ph.VariantType == EVariantType.ObjectId);
            for (var i = 0; i < uniqueOIdCount; i++)
            {
                br.Align(EVariantType.ObjectId.GetAlignment());
            
                var offset = (uint) br.Position();
                var oid = br.ReadUInt64();
            
                OffsetObjectIdMap.Add(offset, oid);
            }
        }
    }
}