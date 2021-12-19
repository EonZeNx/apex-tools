using System.Xml;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.CustomArrays;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class ByteArray : BaseArray<byte>
{
    public override string XmlName => "ByteArray";
    public override EVariantType VariantType => EVariantType.ByteArray;
    public override byte[] Values { get; set; } = Array.Empty<byte>();
    
    
    public ByteArray() { }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var length = Count;
        if (length == -1) length = br.ReadInt32();
        
        var values = new byte[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = br.ReadByte();
        }

        Values = values;
    }

    public override void ToApex(BinaryWriter bw)
    {
        base.ToApex(bw);
        foreach (var value in Values)
        {
            bw.Write(value);
        }
    }

    #endregion
    

    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var byteString = xr.ReadString();
        if (byteString.Length == 0)
        {
            Values = Array.Empty<byte>();
            return;
        }
            
        var bytes = byteString.Split(",");
        Values = Array.ConvertAll(bytes, byte.Parse);
    }
    
    #endregion
}