using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantByteArray : BaseArray<byte>
{
    public override byte[] Value { get; set; } = Array.Empty<byte>();
    public override uint Count { get; set; }
    public override string XmlName => "ByteArray";

    public VariantByteArray() { }
    public VariantByteArray(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();
        
        Value = new byte[Count];
        for (var i = 0; i < Count; i++)
        {
            Value[i] = br.ReadByte();
        }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);

        var array = string.Join(",", Value.Select(ByteUtils.ToHex));
        xw.WriteValue(array);
        xw.WriteEndElement();
    }
}