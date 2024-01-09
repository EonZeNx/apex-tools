using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public class InlineEvents : InlineCountable
{
    public uint NameHash { get; set; }
    public string Name { get; set; } = string.Empty;
    public static EVariantType VariantType => EVariantType.Events;
    protected (uint, uint)[] Values { get; set; } = Array.Empty<(uint, uint)>();
    
    public override uint Count { get; set; }

    public InlineEvents() {}
    public InlineEvents(InlinePropertyHeader header)
    {
        NameHash = header.NameHash;
    }

    public override void LookupHash()
    {
        Name = HashUtils.Lookup(NameHash);
    }

    public override void FromApex(BinaryReader br)
    {
        if (NameHash == 0)
        {
            NameHash = br.ReadUInt32();
            br.ReadByte();
        }

        Count = br.ReadUInt32();
        Values = new (uint, uint)[Count];
        
        for (var i = 0; i < Values.Length; i++)
        {
            Values[i] = (br.ReadUInt32(), br.ReadUInt32());
        }
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((byte) VariantType);
        
        bw.Write((uint) Values.Length);
        foreach (var value in Values)
        {
            bw.Write(value.Item1);
            bw.Write(value.Item2);
        }
    }

    public override XElement ToXElement()
    {
        var xe = new XElement(VariantType.GetXmlName());
        xe.WriteNameOrHash(NameHash, Name);
        
        xe.SetValue(string.Join(",", Values.Select(e => $"{e.Item1:X8}={e.Item2:X8}")));

        return xe;
    }

    public override void FromXElement(XElement xe)
    {
        NameHash = xe.GetNameHash();

        if (string.IsNullOrEmpty(xe.Value))
        {
            Values = Array.Empty<(uint, uint)>();
            Count = 0;
            
            return;
        }
        
        var strValues = xe.Value.Split(",");
        
        Values = new (uint, uint)[strValues.Length];
        for (var i = 0; i < strValues.Length; i++)
        {
            var eventPair = strValues[i].Split("=");
            if (eventPair.Length != 2)
            {
                throw new XmlSchemaException($"Event pair {eventPair} has too many values, not a valid {VariantType.GetXmlName()}");
            }
            
            var eventTuple = (uint.Parse(eventPair[0], NumberStyles.AllowHexSpecifier), uint.Parse(eventPair[1], NumberStyles.AllowHexSpecifier));
            Values[i] = eventTuple;
        }
        
        Count = (uint) Values.Length;
    }
}