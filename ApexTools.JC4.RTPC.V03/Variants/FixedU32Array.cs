using System.Xml;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexTools.Core.Exceptions;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class FixedU32Array : VariantU32Array
{
    public override List<uint> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "F32Array";

    public FixedU32Array() { }
    public FixedU32Array(JC4PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Value = new List<uint>((int) Count);
        for (var i = 0; i < Count; i++)
        {
            Value.Add(br.ReadUInt32());
        }
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var floatString = xr.ReadString();
        if (floatString.Length == 0)
        {
            Value = new List<uint>();
            return;
        }
            
        var floats = floatString.Split(",");
        if (floats.Length != Count)
        {
            throw new MalformedXmlException($"{XmlName} wrong length {Header.NameHash}");
        }
        
        Value = Array.ConvertAll(floats, uint.Parse).ToList();
    }
}