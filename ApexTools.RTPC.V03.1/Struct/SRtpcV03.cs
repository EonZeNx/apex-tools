using System.Xml;
using ApexTools.RTPC.V03._1.Abstractions;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SRtpcHeaderV03"/><br/>
/// Container - <see cref="SContainerV03"/><br/>
/// Variant value maps
/// </summary>
public class SRtpcV03 : IFromApexHeader, IFromApex, IToXml
{
    public SRtpcHeaderV03 Header = new();
    public SContainerV03 Container = new();

    public string XmlName => "RTPC";

    #region Variant maps

    // public Dictionary<uint, string> OffsetToString = new();
    // public Dictionary<uint, float[]> OffsetToVec2 = new();
    // public Dictionary<uint, float[]> OffsetToVec3 = new();
    // public Dictionary<uint, float[]> OffsetToVec4 = new();
    // public Dictionary<uint, float[]> OffsetToMat3X3 = new();
    // public Dictionary<uint, float[]> OffsetToMat4X4 = new();
    // public Dictionary<uint, uint[]> OffsetToU32Array = new();
    // public Dictionary<uint, float[]> OffsetToF32Array = new();
    // public Dictionary<uint, byte[]> OffsetToByteArray = new();
    // public Dictionary<uint, (ulong, byte)> OffsetToObjectId = new();
    // public Dictionary<uint, (uint, uint)[]> OffsetToEvent = new();

    #endregion

    #region IFromApex
    
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
    
    #endregion

    #region IFromApex
    
    public void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        Container.ToXml(xw);
        xw.WriteEndElement();
    }
    
    #endregion
}