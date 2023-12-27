using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ApexTools.JC4.RTPC.V03.NewModels.Utils;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03PropertyHeader
{
    public uint NameHash = 0;
    public byte[] RawData = new byte[4];
    public EVariantType VariantType = EVariantType.Unassigned;

    public string XmlData = string.Empty;
    public string Name = string.Empty;
    
    public static int SizeOf() => 4 + 4 + 1;

    public RtpcV03PropertyHeader(){}
}

public static class RtpcV03PropertyHeaderExtension
{
    public static RtpcV03PropertyHeader ReadRtpcV03PropertyHeader(this BinaryReader br)
    {
        var result = new RtpcV03PropertyHeader
        {
            NameHash = br.ReadUInt32(),
            RawData = br.ReadBytes(4),
            VariantType = (EVariantType) br.ReadByte()
        };

        return result;
    }
    
    public static void LookupNameHash(this ref RtpcV03PropertyHeader propertyHeader)
    {
        propertyHeader.Name = HashUtils.Lookup(propertyHeader.NameHash);
    }

    private static IList<float> ParseF32Array(string data, int count = 0)
    {
        var strValues = data.Split(",");
        var values = Array.ConvertAll(strValues, float.Parse);

        if (count != 0 && values.Length != count)
        {
            throw new Exception("FixedF32Array had incorrect number of elements");
        }

        return values;
    }
    
    private static IList<uint> ParseU32Array(string data, int count = 0)
    {
        var strValues = data.Split(",");
        var values = Array.ConvertAll(strValues, uint.Parse);

        if (count != 0 && values.Length != count)
        {
            throw new Exception("U32Array had incorrect number of elements");
        }

        return values;
    }
    
    private static IList<byte> ParseByteArray(string data, int count = 0)
    {
        var strValues = data.Split(",");
        var values = strValues
            .Select(s => byte.Parse(s, NumberStyles.HexNumber))
            .ToArray();

        if (count != 0 && values.Length != count)
        {
            throw new Exception("ByteArray had incorrect number of elements");
        }

        return values;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03PropertyHeader header, RtpcV03ValueOffsetMaps voMaps)
    {
        bw.Write(header.NameHash);
        switch (header.VariantType)
        {
            case EVariantType.UInteger32:
                bw.Write(BitConverter.ToUInt32(header.RawData));
                break;
            case EVariantType.Float32:
                bw.Write(BitConverter.ToSingle(header.RawData));
                break;
            case EVariantType.String:
                bw.Write(voMaps.StringOffsetMap[header.XmlData]);
                break;
            case EVariantType.Vector2:
                var vec2Values = ParseF32Array(header.XmlData, 2);
                bw.Write(voMaps.Vec2OffsetMap[vec2Values]);
                break;
            case EVariantType.Vector3:
                var vec3Values = ParseF32Array(header.XmlData, 3);
                bw.Write(voMaps.Vec3OffsetMap[vec3Values]);
                break;
            case EVariantType.Vector4:
                var vec4Values = ParseF32Array(header.XmlData, 4);
                bw.Write(voMaps.Vec4OffsetMap[vec4Values]);
                break;
            case EVariantType.Matrix3X3:
                var mat3Values = ParseF32Array(header.XmlData, 9);
                bw.Write(voMaps.Mat3OffsetMap[mat3Values]);
                break;
            case EVariantType.Matrix4X4:
                var mat4Values = ParseF32Array(header.XmlData, 16);
                bw.Write(voMaps.Mat4OffsetMap[mat4Values]);
                break;
            case EVariantType.UInteger32Array:
                var u32Values = ParseU32Array(header.XmlData);
                bw.Write(voMaps.U32ArrayOffsetMap[u32Values]);
                break;
            case EVariantType.Float32Array:
                var f32Values = ParseF32Array(header.XmlData);
                bw.Write(voMaps.F32ArrayOffsetMap[f32Values]);
                break;
            case EVariantType.ByteArray:
                var byteValues = ParseByteArray(header.XmlData);
                bw.Write(voMaps.ByteArrayOffsetMap[byteValues]);
                break;
            case EVariantType.ObjectId:
                var oidValue = ulong.Parse(header.XmlData, NumberStyles.HexNumber);
                bw.Write(voMaps.ObjectIdOffsetMap[oidValue]);
                break;
            case EVariantType.Event:
                var strValues = header.XmlData.Split(",");
                var eventArray = strValues
                    .Select(sv => sv.Split("=")
                        .Select(s => uint.Parse(s, NumberStyles.HexNumber)).ToArray())
                    .Select(events => (events[0], events[1]))
                    .ToList();
                bw.Write(voMaps.EventOffsetMap[eventArray]);
                break;
            case EVariantType.Unassigned:
            case EVariantType.Deprecated:
            case EVariantType.Total:
            default:
                bw.Write(header.RawData);
                break;
        }
        bw.Write((byte) header.VariantType);
    }

    public static void Write(this XElement pxe, RtpcV03PropertyHeader header, in RtpcV03OffsetValueMaps ovMaps)
    {
        if (Settings.SkipUnassignedRtpcProperties.Value && header.VariantType == EVariantType.Unassigned)
        {
            return;
        }
        
        var xe = new XElement(header.VariantType.GetXmlName());
        xe.WriteNameOrHash(header.NameHash, header.Name);

        var uint32Data = BitConverter.ToUInt32(header.RawData);
        switch (header.VariantType)
        {
            case EVariantType.Unassigned:
                break;
            case EVariantType.UInteger32:
                xe.SetValue(uint32Data);
                break;
            case EVariantType.Float32:
                var floatValue = BitConverter.ToSingle(header.RawData);
                xe.SetValue(floatValue);
                break;
            case EVariantType.String:
                xe.SetValue(ovMaps.OffsetStringMap[uint32Data]);
                break;
            case EVariantType.Vector2:
                var vector2 = ovMaps.OffsetVec2Map[uint32Data];
                xe.SetValue(string.Join(",", vector2));
                break;
            case EVariantType.Vector3:
                var vector3 = ovMaps.OffsetVec3Map[uint32Data];
                xe.SetValue(string.Join(",", vector3));
                break;
            case EVariantType.Vector4:
                var vector4 = ovMaps.OffsetVec4Map[uint32Data];
                xe.SetValue(string.Join(",", vector4));
                break;
            case EVariantType.Matrix3X3:
                var matrix3 = ovMaps.OffsetMat3X3Map[uint32Data];
                xe.SetValue(string.Join(",", matrix3));
                break;
            case EVariantType.Matrix4X4:
                var matrix4 = ovMaps.OffsetMat4X4Map[uint32Data];
                xe.SetValue(string.Join(",", matrix4));
                break;
            case EVariantType.UInteger32Array:
                var uint32Array = ovMaps.OffsetU32ArrayMap[uint32Data];
                xe.SetValue(string.Join(",", uint32Array));
                break;
            case EVariantType.Float32Array:
                var floatArray = ovMaps.OffsetF32ArrayMap[uint32Data];
                xe.SetValue(string.Join(",", floatArray));
                break;
            case EVariantType.ByteArray:
                var byteArray = ovMaps.OffsetByteArrayMap[uint32Data];
                xe.SetValue(string.Join(",", byteArray.Select(b => $"{b:X2}")));
                break;
            case EVariantType.Deprecated:
                break;
            case EVariantType.ObjectId:
                var objectId = ovMaps.OffsetObjectIdMap[uint32Data];
                xe.SetValue($"{objectId:X16}");
                break;
            case EVariantType.Event:
                var eventPairArray = ovMaps.OffsetEventMap[uint32Data];
                var eventPairStringArray = eventPairArray.Select(ep => $"{ep.Item1:X8}={ep.Item2:X8}");
                var eventString = string.Join(",", eventPairStringArray);
                xe.SetValue(eventString);
                break;
            case EVariantType.Total:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        pxe.Add(xe);
    }

    public static RtpcV03PropertyHeader ReadRtpcV03PropertyHeader(this XElement xe)
    {
        var result = new RtpcV03PropertyHeader
        {
            NameHash = xe.GetNameHash(),
            VariantType = xe.GetVariant()
        };

        switch (result.VariantType)
        {
            case EVariantType.UInteger32:
                result.RawData = BitConverter.GetBytes(uint.Parse(xe.Value));
                break;
            case EVariantType.Float32:
                result.RawData = BitConverter.GetBytes(float.Parse(xe.Value));
                break;
            case EVariantType.Unassigned:
            case EVariantType.String:
            case EVariantType.Vector2:
            case EVariantType.Vector3:
            case EVariantType.Vector4:
            case EVariantType.Matrix3X3:
            case EVariantType.Matrix4X4:
            case EVariantType.UInteger32Array:
            case EVariantType.Float32Array:
            case EVariantType.ByteArray:
            case EVariantType.Deprecated:
            case EVariantType.ObjectId:
            case EVariantType.Event:
            case EVariantType.Total:
            default:
                result.XmlData = xe.Value;
                break;
        }

        return result;
    }
}