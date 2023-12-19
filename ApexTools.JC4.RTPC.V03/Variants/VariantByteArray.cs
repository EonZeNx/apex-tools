using System.Xml;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantByteArray : ABaseArray<byte>
{
    public override List<byte> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "ByteArray";

    public VariantByteArray()
    {
        Header.VariantType = EVariantType.ByteArray;
    }
    public VariantByteArray(JC4PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();

        Value = new List<byte>((int) Count);
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

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var byteString = xr.ReadString();
        if (byteString.Length == 0)
        {
            Value = new List<byte>();
            return;
        }
            
        var bytes = byteString.Split(",");
        Value = Array.ConvertAll(bytes, ByteUtils.HexToByte).ToList();
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