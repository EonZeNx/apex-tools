using System.Xml;
using EonZeNx.ApexFormats.RTPC.V01.Models.Properties.CustomArrays;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class ByteArray : BaseArray<byte>
{
    public override string XmlName => "ByteArray";
    public override EVariantType VariantType => EVariantType.ByteArray;
    public override bool Primitive => false;
    public override int Alignment => 4;
    public override byte[] Values { get; set; } = Array.Empty<byte>();
    
    
    public ByteArray() { }
    public ByteArray(RtpcV01PropertyHeader header) : base(header) { }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var dataOffset = BitConverter.ToUInt32(RawData);
            
        br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

        var length = Count;
        if (length == -1) length = br.ReadInt32();
        
        var values = new byte[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = br.ReadByte();
        }

        Values = values;
    }

    public override void ToApexDeferred(BinaryWriter bw)
    {
        base.ToApexDeferred(bw);
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