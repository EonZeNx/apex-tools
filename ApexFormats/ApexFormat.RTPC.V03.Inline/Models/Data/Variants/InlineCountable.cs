using System.Xml.Linq;
using ApexFormat.RTPC.V03.Inline.Interfaces;

namespace ApexFormat.RTPC.V03.Inline.Models.Data.Variants;

public abstract class InlineCountable : IApexXElementIO
{
    public abstract uint Count { get; set; }

    public abstract void LookupHash();
    
    public abstract void FromApex(BinaryReader br);
    public abstract void ToApex(BinaryWriter bw);
    public abstract XElement ToXElement();
}