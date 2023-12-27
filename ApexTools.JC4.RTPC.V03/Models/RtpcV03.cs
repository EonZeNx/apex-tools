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
public class RtpcV03 : IFromApex, IToXml, IFromXml, IToApex
{
    public RtpcHeaderV03 Header = new();
    public RootContainerV03 RootContainer = new();

    public string XmlName => "RTPC";
    public string Extension { get; set; } = ".rtpc";

    #region Variant maps

    protected readonly ValueOffsetMapV03<string, VariantStr> StringOffsetMap = new(EVariantType.String, 0);
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec2> Vec2OffsetMap = new(EVariantType.Vector2, new ListEqualityComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec3> Vec3OffsetMap = new(EVariantType.Vector3, new ListEqualityComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantVec4> Vec4OffsetMap = new(EVariantType.Vector4, new ListEqualityComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantMat3X3> Mat3X3OffsetMap = new(EVariantType.Matrix3X3, new ListEqualityComparer<float>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantMat4X4> Mat4X4OffsetMap = new(EVariantType.Matrix4X4, new ListEqualityComparer<float>(), 16);
    protected readonly ValueOffsetMapV03<IList<uint>, VariantU32Array> U32ArrayOffsetMap = new(EVariantType.UInteger32Array, new ListEqualityComparer<uint>());
    protected readonly ValueOffsetMapV03<IList<float>, VariantF32Array> F32ArrayOffsetMap = new(EVariantType.Float32Array, new ListEqualityComparer<float>());
    protected readonly ValueOffsetMapV03<IList<byte>, VariantByteArray> ByteArrayOffsetMap = new(EVariantType.ByteArray, new ListEqualityComparer<byte>(), 16);
    protected readonly ValueOffsetMapV03<(ulong, byte), VariantObjectId> ObjectIdOffsetMap = new(EVariantType.ObjectId, new U64BComparer());
    protected readonly ValueOffsetMapV03<IList<(uint, uint)>, VariantEvent> EventOffsetMap = new(EVariantType.Event, new ListEqualityComparer<(uint, uint)>());

    protected readonly OffsetValueMaps OvMaps = new();

    #endregion

    public void CreateValueMaps(APropertyV03[] properties, BinaryWriter bw)
    {
        // Order is specific
        StringOffsetMap.Create(properties, bw);
        Vec2OffsetMap.Create(properties, bw);
        Vec3OffsetMap.Create(properties, bw);
        Vec4OffsetMap.Create(properties, bw);
        Mat3X3OffsetMap.Create(properties, bw);
        Mat4X4OffsetMap.Create(properties, bw);
        U32ArrayOffsetMap.Create(properties, bw);
        F32ArrayOffsetMap.Create(properties, bw);
        ByteArrayOffsetMap.Create(properties, bw);
        EventOffsetMap.Create(properties, bw);
        ObjectIdOffsetMap.Create(properties, bw);
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
        // var allPropertyHeaders = RootContainer.GetAllPropertyHeaders();
        // var uniqueOffsets = allPropertyHeaders
        //     .Where(ph => ph.VariantType is not
        //         (EVariantType.Unassigned or EVariantType.UInteger32 or EVariantType.Float32))
        //     .GroupBy(ph => BitConverter.ToUInt32(ph.RawData))
        //     .Select(g => g.First())
        //     .ToArray();
        //
        // var originalOffset = br.Position();
        // OvMaps.Create(br, uniqueOffsets);
        // br.Seek(originalOffset);
        
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
        xr.ReadToFollowing(XmlName);
        Extension = xr.GetAttribute(nameof(Extension)) ?? Extension;
        xr.ReadToDescendant(RootContainer.XmlName);
        
        RootContainer.FromXml(xr);
    }

    #endregion
}