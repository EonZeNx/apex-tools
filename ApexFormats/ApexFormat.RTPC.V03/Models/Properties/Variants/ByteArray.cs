using System.Xml;
using ApexFormat.RTPC.V03.Models.Properties.CustomArrays;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models.Properties.Variants;

public class ByteArray : BaseArray<byte>
{
    public override string XmlName => "ByteArray";
    public override EVariantType VariantType => EVariantType.ByteArray;
    public override bool Primitive => false;
    public override int Alignment => 4;
    public override byte[] Values { get; set; } = Array.Empty<byte>();
    
    
    public ByteArray() { }
    public ByteArray(PropertyHeaderV03 header) : base(header) { }


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
        NameHash = xr.ReadNameIfValid();
            
        var byteString = xr.ReadElementContentAsString();
        if (byteString.Length == 0)
        {
            Values = Array.Empty<byte>();
            return;
        }
            
        var bytes = byteString.Split(",");
        Values = Array.ConvertAll(bytes, ByteUtils.HexToByte);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        xw.WriteNameOrNameHash(NameHash, Name);

        var array = string.Join(",", Values.Select(ByteUtils.ToHex));
        xw.WriteValue(array);
        xw.WriteEndElement();
    }

    #endregion
}