using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class Event : PropertyBaseDeferredV01
{
    public override string XmlName => "Event";
    public override EVariantType VariantType => EVariantType.Event;
    public override bool Primitive => false;
    public override int Alignment => 4;

    public (uint, uint)[] Value { get; set; } = Array.Empty<(uint, uint)>();


    public Event() { }
    public Event(PropertyHeaderV01 header)
    {
        NameHash = header.NameHash;
        RawData = header.RawData;
    }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        var dataOffset = BitConverter.ToUInt32(RawData);
            
        br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
        var length = br.ReadUInt32();
        Value = new (uint, uint)[length];
        for (var i = 0; i < length; i++)
        {
            var firstEventHalf = br.ReadUInt32();
            var secondEventHalf = br.ReadUInt32();
            Value[i] = (firstEventHalf, secondEventHalf);
        }
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((uint) Offset);
        bw.Write((byte) VariantType);
    }
    
    public override void ToApexDeferred(BinaryWriter bw)
    {
        bw.Align(Alignment);
        Offset = bw.Position();
        
        // TODO: Check this skips the for loop on zero length array
        bw.Write(Value.Length);
        for (var i = 0; i < Value.Length; i++)
        {
            bw.Write(Value[i].Item1);
            bw.Write(Value[i].Item2);
        }
    }

    #endregion

    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = Array.Empty<(uint, uint)>();
        
        var value = xr.ReadString();
        if (value.Length == 0) return;

        string[] eventStringArray = {value};
        if (value.Contains(','))
        {
            eventStringArray = value.Split(", ");
        }
        
        Value = (from eventString in eventStringArray 
                select eventString.Split("=") into eventStrings 
                select Array.ConvertAll(eventStrings, ByteUtils.HexToUint) into eventsArray 
                select (eventsArray[0], eventsArray[1]))
            .ToArray();

        // foreach (var eventString in eventStringArray)
        // {
        //     var eventStrings = eventString.Split("=");
        //     var eventsArray = Array.ConvertAll(eventStrings, ByteUtils.HexToUint);
        //     var eventsTuple = (eventsArray[0], eventsArray[1]);
        //         
        //     events.Add(eventsTuple);
        // }
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
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
    
    #endregion
}