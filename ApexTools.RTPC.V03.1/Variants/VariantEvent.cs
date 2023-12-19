using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantEvent : BaseArray<(uint, uint)>
{
    public override (uint, uint)[] Value { get; set; } = Array.Empty<(uint, uint)>();
    public override uint Count { get; set; }
    public override string XmlName => "Event";

    public VariantEvent() { }
    public VariantEvent(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();
        
        Value = new (uint, uint)[Count];
        for (var i = 0; i < Count; i++)
        {
            var first = br.ReadUInt32();
            var second = br.ReadUInt32();
            Value[i] = (first, second);
        }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);
            
        var strArray = new string[Value.Length];
        for (var i = 0; i < Value.Length; i++)
        {
            var item1 = ByteUtils.ToHex(Value[i].Item1);
            var item2 = ByteUtils.ToHex(Value[i].Item2);
            strArray[i] = $"{item1}={item2}";
        }

        var array = string.Join(", ", strArray);
        xw.WriteValue(array);
        xw.WriteEndElement();
    }
}