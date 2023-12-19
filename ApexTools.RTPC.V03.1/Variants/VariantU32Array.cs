using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantU32Array : BaseArray<uint>
{
    public override uint[] Value { get; set; } = Array.Empty<uint>();
    public override uint Count { get; set; }
    public override string XmlName => "U32Array";

    public VariantU32Array() { }
    public VariantU32Array(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();
        
        Value = new uint[Count];
        for (var i = 0; i < Count; i++)
        {
            Value[i] = br.ReadUInt32();
        }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);

        var array = string.Join(",", Value);
        xw.WriteValue(array);
        xw.WriteEndElement();
    }
}