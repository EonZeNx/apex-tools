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
                br.Align(4);

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
                br.Align(4);

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
                br.Align(4);

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
                br.Align(4);

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
                br.Align(16);

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
                br.Align(4);

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
                br.Align(4);

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
                br.Align(16);

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
                br.Align(4);

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
                br.Align(4);
            
                var offset = (uint) br.Position();
                var oid = br.ReadUInt64();
            
                OffsetObjectIdMap.Add(offset, oid);
            }
        }
    }
}