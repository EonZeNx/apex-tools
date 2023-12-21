using System.Xml;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantF32Array : ABaseArray<float>
{
    public override List<float> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "F32Array";

    public VariantF32Array()
    {
        Header.VariantType = EVariantType.Float32Array;
    }
    public VariantF32Array(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();

        Value = new List<float>((int) Count);
        for (var i = 0; i < Count; i++)
        {
            Value.Add(br.ReadSingle());
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

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
            
        var floatString = xr.ReadString();
        if (floatString.Length == 0)
        {
            Value = new List<float>();
            return;
        }
            
        var floats = floatString.Split(",");
        Value = Array.ConvertAll(floats, float.Parse).ToList();
        Count = (uint) Value.Count;
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value.Count);
        foreach (var value in Value)
        {
            bw.Write(value);
        }
    }
}