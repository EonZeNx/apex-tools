using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Event : BasePropertyV01
{
    public override string XmlName => "Event";
    
    protected override EVariantType VariantType => EVariantType.Event;
    protected (uint, uint)[] Value { get; set; } = Array.Empty<(uint, uint)>();


    public Event() { }
    public Event(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01) { }
    
    
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

        var value = xr.ReadElementContentAsString();
        if (value.Length == 0)
        {
            Value = Array.Empty<(uint, uint)>();
            return;
        }

        string[] eventStringArray = {value};
        if (value.Contains(','))
        {
            eventStringArray = value.Split(", ");
        }

        Value = (from eventString in eventStringArray 
            select eventString.Split("=") into eventStrings 
            select Array.ConvertAll(eventStrings, ByteUtils.HexToUInt) into eventsArray 
            select (eventsArray[0], eventsArray[1])
        ).ToArray();
        
        // var events = new List<(uint, uint)>();
        // foreach (var eventString in eventStringArray)
        // {
        //     var eventStrings = eventString.Split("=");
        //     var eventsArray = Array.ConvertAll(eventStrings, ByteUtils.HexToUint);
        //     var eventsTuple = (eventsArray[0], eventsArray[1]);
        //         
        //     events.Add(eventsTuple);
        // }
        //
        // Value = events.ToArray();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        xw.WriteAttributeString("Offset", ByteUtils.ToHex((uint) Offset));
            
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