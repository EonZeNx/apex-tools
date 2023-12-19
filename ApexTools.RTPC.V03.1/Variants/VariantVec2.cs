using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantVec2 : BaseArray<float>
{
    public override float[] Value { get; set; } = Array.Empty<float>();
    public override uint Count { get; set; } = 2;
    public override string XmlName => "Vec2";

    public VariantVec2() { }
    public VariantVec2(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);
        
        Value = new float[Count];
        for (var i = 0; i < Count; i++)
        {
            Value[i] = br.ReadSingle();
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