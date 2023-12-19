using System.Xml;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantU32Array : ABaseArray<uint>
{
    public override List<uint> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "U32Array";

    public VariantU32Array()
    {
        Header.VariantType = EVariantType.UInteger32Array;
    }
    public VariantU32Array(JC4PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();
        
        Value = new List<uint>((int) Count);
        for (var i = 0; i < Count; i++)
        {
            Value.Add(br.ReadUInt32());
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
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var uintString = xr.ReadString();
        if (uintString.Length == 0)
        {
            Value = new List<uint>();
            return;
        }
        
        var uints = uintString.Split(",");
        Value = Array.ConvertAll(uints, uint.Parse).ToList();
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