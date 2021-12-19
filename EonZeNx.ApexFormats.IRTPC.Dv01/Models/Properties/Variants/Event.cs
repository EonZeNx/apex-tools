using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class Event : PropertyBase
{
    public override string XmlName => "Event";
    public override EVariantType VariantType => EVariantType.Event;

    public (uint, uint)[] Value { get; set; } = Array.Empty<(uint, uint)>();


    public Event() { }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
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
        base.ToApex(bw);
        bw.Write((uint) Value.Length);
        
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