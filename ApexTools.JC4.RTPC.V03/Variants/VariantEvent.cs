using System.Xml;
using ApexTools.JC4.RTPC.V03.Models;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantEvent : ABaseArray<(uint, uint)>
{
    public override List<(uint, uint)> Value { get; set; } = new();
    public override uint Count { get; set; }
    public override string XmlName => "Event";

    public VariantEvent()
    {
        Header.VariantType = EVariantType.Event;
    }
    public VariantEvent(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {
        var offset = BitConverter.ToUInt32(Header.RawData);
        br.Seek(offset);

        Count = br.ReadUInt32();
        
        Value = new List<(uint, uint)>((int) Count);
        for (var i = 0; i < Count; i++)
        {
            var first = br.ReadUInt32();
            var second = br.ReadUInt32();
            Value.Add((first, second));
        }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);
            
        var strArray = new string[Value.Count];
        for (var i = 0; i < Value.Count; i++)
        {
            var item1 = ByteUtils.ToHex(Value[i].Item1);
            var item2 = ByteUtils.ToHex(Value[i].Item2);
            strArray[i] = $"{item1}={item2}";
        }

        var array = string.Join(", ", strArray);
        xw.WriteValue(array);
        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
        Value = new List<(uint, uint)>();
        
        var value = xr.ReadString();
        if (value.Length == 0) return;

        string[] eventPairStringArray = {value};
        if (value.Contains(','))
        {
            eventPairStringArray = value.Split(", ");
        }
        
        Value = (from eventString in eventPairStringArray 
                select eventString.Split("=") into eventStrings 
                select Array.ConvertAll(eventStrings, ByteUtils.HexToUInt) into eventsArray 
                select (eventsArray[0], eventsArray[1]))
            .ToList();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value.Count);
        for (var i = 0; i < Value.Count; i++)
        {
            bw.Write(Value[i].Item1);
            bw.Write(Value[i].Item2);
        }
    }
}