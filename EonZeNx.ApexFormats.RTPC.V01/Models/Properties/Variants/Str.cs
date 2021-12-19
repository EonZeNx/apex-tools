using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

public class Str : PropertyBaseDeferredV01
{
    public override string XmlName => "String";
    public override EVariantType VariantType => EVariantType.String;
    public override int Alignment => 0;
    public override bool Primitive => false;

    public static readonly Dictionary<string, long> StringMap = new();
    public string Value { get; set; } = "";


    public Str() { }
    public Str(PropertyHeaderV01 header)
    {
        NameHash = header.NameHash;
        RawData = header.RawData;
    }


    #region ApexSerializable
    
    public override void FromApex(BinaryReader br)
    {
        var dataOffset = BitConverter.ToUInt32(RawData);
        br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
        
        var byteString = new List<byte>();
        while (true)
        {
            var thisByte = br.ReadByte();
            if (thisByte.Equals(0x00)) break;
                
            byteString.Add(thisByte);
        }
        Value = Encoding.UTF8.GetString(byteString.ToArray());
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((uint) Offset);
        bw.Write((byte) VariantType);
    }
    
    public override void ToApexDeferred(BinaryWriter bw)
    {
        // If value already exists in file, use that offset
        if (StringMap.ContainsKey(Value))
        {
            Offset = StringMap[Value];
            return;
        }
            
        bw.Align(Alignment);
        Offset = bw.Position();
        StringMap[Value] = Offset;
            
        bw.Write(Encoding.UTF8.GetBytes(Value));
        bw.Write((byte) 0x00);
    }

    #endregion

    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = xr.ReadString();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    #endregion
}