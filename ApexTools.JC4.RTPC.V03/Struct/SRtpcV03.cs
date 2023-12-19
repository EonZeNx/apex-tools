using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.ValueOffsetMap;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SRtpcHeaderV03"/><br/>
/// Container - <see cref="SContainerV03"/><br/>
/// Variant value maps
/// </summary>
public class SRtpcV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApex
{
    public SRtpcHeaderV03 Header = new();
    public SContainerV03 Container = new();

    public string XmlName => "RTPC";

    #region Variant maps

    protected readonly ValueToOffsetMapV03<string, VariantStr> StringToOffsetMap = new(EVariantType.String, 0);
    protected readonly F32ListToOffsetMapV03<VariantVec2> Vec2ToOffsetMap = new(EVariantType.Vec2);
    protected readonly F32ListToOffsetMapV03<VariantVec3> Vec3ToOffsetMap = new(EVariantType.Vec3);
    protected readonly F32ListToOffsetMapV03<VariantVec4> Vec4ToOffsetMap = new(EVariantType.Vec4);
    protected readonly F32ListToOffsetMapV03<VariantMat3X3> Mat3X3ToOffsetMap = new(EVariantType.Mat3X3);
    protected readonly F32ListToOffsetMapV03<VariantMat4X4> Mat4X4ToOffsetMap = new(EVariantType.Mat4X4, 16);
    protected readonly ValueToOffsetMapV03<IList<uint>, VariantU32Array> U32ArrayToOffsetMap = new(EVariantType.UInteger32Array);
    protected readonly F32ListToOffsetMapV03<VariantF32Array> F32ArrayToOffsetMap = new(EVariantType.Float32Array);
    protected readonly ValueToOffsetMapV03<IList<byte>, VariantByteArray> ByteArrayToOffsetMap = new(EVariantType.ByteArray, 16);
    protected readonly OIdToOffsetMapV03<VariantObjectId> ObjectIdToOffsetMap = new(EVariantType.ObjectId);
    protected readonly ValueToOffsetMapV03<IList<(uint, uint)>, VariantEvent> EventToOffsetMap = new(EVariantType.Event);

    #endregion

    #region IApex
    
    public void FromApexHeader(BinaryReader br)
    {
        Header = new SRtpcHeaderV03();
        Header.FromApexHeader(br);

        Container = new SContainerV03();
        Container.Header.FromApexHeader(br);
        Container.FromApexHeader(br);
    }

    public void FromApex(BinaryReader br)
    {
        Container.FromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        var allProperties = Container.GetAllProperties().ToArray();
        var allContainers = Container.GetAllContainers().ToArray();
        
        var propertyDataOffset = SRtpcHeaderV03.BinarySize + 
                         allProperties.Length * JC4PropertyHeaderV03.BinarySize + 
                         allContainers.Length * SContainerHeaderV03.BinarySize +
                         allContainers.Length * 4;

        bw.Seek(propertyDataOffset, SeekOrigin.Begin);
        CreateValueMaps(allProperties, bw);
        
        bw.Seek(0, SeekOrigin.Begin);
        Header.ToApexHeader(bw);
        Container.Header.ToApexHeader(bw);
        Container.ToApexHeader(bw);
    }

    #endregion

    #region IApex utils

    public void CreateValueMaps(APropertyV03[] properties, BinaryWriter bw)
    {
        StringToOffsetMap.Create(properties, bw);
        Vec2ToOffsetMap.Create(properties, bw);
        Vec3ToOffsetMap.Create(properties, bw);
        Vec4ToOffsetMap.Create(properties, bw);
        Mat3X3ToOffsetMap.Create(properties, bw);
        Mat4X4ToOffsetMap.Create(properties, bw);
        // U32ArrayToOffsetMap.Create(properties, bw);
        // F32ArrayToOffsetMap.Create(properties, bw);
        // ByteArrayToOffsetMap.Create(properties, bw);
        // ObjectIdToOffsetMap.Create(properties, bw);
        // EventToOffsetMap.Create(properties, bw);
        
        // TODO: Byte array contains list of object ids
        // Reference those oids as offsets instead of writing the values again
        
        // TODO: Look into generating the 4 properties in the root container from XML structure
        // (Once FromApex ToXml structure is regenerated like JC3 layout)
    }
    
    #endregion

    #region IXml
    
    public void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        Container.ToXml(xw);
        xw.WriteEndElement();
    }

    public void FromXml(XmlReader xr)
    {
        xr.ReadToDescendant(Container.XmlName);
        Container.FromXml(xr);
    }

    #endregion
}