using System.Xml;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexTools.Core.Exceptions;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class FixedF32Array : VariantF32Array
{
    public override List<float> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "F32Array";

    public FixedF32Array() { }
    public FixedF32Array(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);
        
        Value = new List<float>((int) Count);
        for (var i = 0; i < Count; i++)
        {
            Value.Add(br.ReadSingle());
        }
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
        if (floats.Length != Count)
        {
            throw new MalformedXmlException($"{XmlName} wrong length {Header.NameHash}");
        }
        
        Value = Array.ConvertAll(floats, float.Parse).ToList();
    }

    public override void ToApex(BinaryWriter bw)
    {
        foreach (var value in Value)
        {
            bw.Write(value);
        }
    }
}