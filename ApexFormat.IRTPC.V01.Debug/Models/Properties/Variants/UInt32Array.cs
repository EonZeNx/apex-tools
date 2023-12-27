using System.Xml;
using ApexFormat.IRTPC.V01.Debug.Models.Properties.CustomArrays;
using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Debug.Models.Properties.Variants;

public class UInt32Array : BaseArray<uint>
{
    public override string XmlName => "UInt32Array";
    public override EVariantType VariantType => EVariantType.UInteger32Array;
    public override uint[] Values { get; set; } = Array.Empty<uint>();
    
    
    public UInt32Array() { }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var length = Count;
        if (length == -1) length = br.ReadInt32();
        
        var values = new uint[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = br.ReadUInt32();
        }

        Values = values;
    }

    #endregion
    

    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var uintString = xr.ReadElementContentAsString();
        if (uintString.Length == 0)
        {
            Values = Array.Empty<uint>();
            return;
        }
            
        var uints = uintString.Split(",");
        Values = Array.ConvertAll(uints, uint.Parse);
    }
    
    #endregion
}