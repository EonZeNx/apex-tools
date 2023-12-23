using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.ValueOffsetMap;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Models;

/// <summary>
/// Format:<br/>
/// Header - <see cref="RtpcHeaderV03"/><br/>
/// Container - <see cref="ContainerV03"/><br/>
/// Variant value maps
/// </summary>
public class RtpcV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApex
{
    public RtpcHeaderV03 Header = new();
    public RootContainerV03 RootContainer = new();

    public string XmlName => "RTPC";
    public string Extension { get; set; } = ".rtpc";

    #region Variant maps

    protected readonly ValueOffsetMapV03<string, VariantStr> StringOffsetMap = new(EVariantType.String, 0);
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec2> Vec2OffsetMap = new(EVariantType.Vector2, new ListComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec3> Vec3OffsetMap = new(EVariantType.Vector3, new ListComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec4> Vec4OffsetMap = new(EVariantType.Vector4, new ListComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantMat3X3> Mat3X3OffsetMap = new(EVariantType.Matrix3X3, new ListComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantMat4X4> Mat4X4OffsetMap = new(EVariantType.Matrix4X4, new ListComparer<float>(), 16);
    protected readonly ValueOffsetMapV03<IList<uint>, VariantU32Array> U32ArrayOffsetMap = new(EVariantType.UInteger32Array, new ListComparer<uint>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantF32Array> F32ArrayOffsetMap = new(EVariantType.Float32Array, new ListComparer<float>());
    protected readonly ValueOffsetMapV03<IList<byte>, VariantByteArray> ByteArrayOffsetMap = new(EVariantType.ByteArray, new ListComparer<byte>(), 16);
    protected readonly ValueOffsetMapV03<(ulong, byte), VariantObjectId> ObjectIdOffsetMap = new(EVariantType.ObjectId, new U64BComparer());
    protected readonly ValueOffsetMapV03<IList<(uint, uint)>, VariantEvent> EventOffsetMap = new(EVariantType.Event, new ListComparer<(uint, uint)>());

    protected readonly Dictionary<uint, string> OffsetStringMap = new();
    protected readonly Dictionary<uint, IList<float>> OffsetVec2Map = new();
    protected readonly Dictionary<uint, IList<float>> OffsetVec3Map = new();
    protected readonly Dictionary<uint, IList<float>> OffsetVec4Map = new();
    protected readonly Dictionary<uint, IList<float>> OffsetMat3X3Map = new();
    protected readonly Dictionary<uint, IList<float>> OffsetMat4X4Map = new();
    protected readonly Dictionary<uint, IList<uint>> OffsetU32ArrayMap = new();
    protected readonly Dictionary<uint, IList<float>> OffsetF32Map = new();
    protected readonly Dictionary<uint, IList<byte>> OffsetByteMap = new();
    protected readonly Dictionary<uint, (ulong, byte)> OffsetObjectIdMap = new();
    protected readonly Dictionary<uint, IList<(uint, uint)>> OffsetEventMap = new();

    #endregion

    public void CreateValueMaps(APropertyV03[] properties, BinaryWriter bw)
    {
        StringOffsetMap.Create(properties, bw);
        Vec2OffsetMap.Create(properties, bw);
        Vec3OffsetMap.Create(properties, bw);
        Vec4OffsetMap.Create(properties, bw);
        Mat3X3OffsetMap.Create(properties, bw);
        Mat4X4OffsetMap.Create(properties, bw);
        U32ArrayOffsetMap.Create(properties, bw);
        F32ArrayOffsetMap.Create(properties, bw);
        ByteArrayOffsetMap.Create(properties, bw);
        ObjectIdOffsetMap.Create(properties, bw);
        EventOffsetMap.Create(properties, bw);
        
        // TODO: Byte array contains list of object ids
        // Reference those oids as offsets instead of writing the values again
    }

    public void ReadOffsetMap()
    {
        
    }
    
    #region IApex
    
    public void FromApexHeader(BinaryReader br)
    {
        Header = new RtpcHeaderV03();
        Header.FromApex(br);

        RootContainer = new RootContainerV03();
        RootContainer.Header.FromApex(br);
        RootContainer.FromApexHeader(br);
    }

    public void FromApex(BinaryReader br)
    {
        // Get all properties
        // For each variant
        // Read value map
        var allProperties = RootContainer.GetAllProperties();
        var uniqueOffsets = allProperties
            .Where(p => p.Header.VariantType is not
                (EVariantType.Unassigned or EVariantType.UInteger32 or EVariantType.Float32))
            .GroupBy(p => BitConverter.ToUInt32(p.Header.RawData))
            .Select(g => g.First());
        
        RootContainer.FromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        RootContainer.Flatten();
        
        var allProperties = RootContainer.GetAllProperties().ToArray();
        // RootContainer doesn't count itself
        var containerCount = 1 + RootContainer.GetContainerCount();
        
        var propertyDataOffset = (uint) (RtpcHeaderV03.BinarySize + 
                                        allProperties.Length * PropertyHeaderV03.BinarySize + 
                                        containerCount * ContainerHeaderV03.BinarySize +
                                        containerCount * 4);
        
        bw.Seek((int) propertyDataOffset, SeekOrigin.Begin);
        CreateValueMaps(allProperties, bw);
        
        bw.Seek(0, SeekOrigin.Begin);
        Header.ToApex(bw);
        RootContainer.Header.ToApex(bw);
        RootContainer.ToApexHeader(bw);
    }

    #endregion

    #region IXml
    
    public void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        xw.WriteAttributeString(nameof(Extension), Extension);
        xw.WriteAttributeString(nameof(Header.Version), $"{Header.Version}");
        
        RootContainer.ToXml(xw);
        xw.WriteEndElement();
    }

    public void FromXml(XmlReader xr)
    {
        xr.ReadToDescendant(RootContainer.XmlName);
        Extension = xr.GetAttribute(nameof(Extension)) ?? Extension;
        
        RootContainer.FromXml(xr);
    }

    #endregion
}