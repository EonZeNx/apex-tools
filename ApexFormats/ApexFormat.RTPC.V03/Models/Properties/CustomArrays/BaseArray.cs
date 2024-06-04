using System.Xml;
using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Models.Properties.CustomArrays;

public abstract class BaseArray<T> : PropertyBaseDeferredV03
{
    public abstract T[] Values { get; set; }
    public virtual int Count { get; set; } = -1;
    public override int Alignment => 4;
    public override bool Primitive => false;


    public BaseArray() { }
    public BaseArray(PropertyHeaderV03 header)
    {
        NameHash = header.NameHash;
        RawData = header.RawData;
    }


    #region ApexSerializable

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write((uint) Offset);
        bw.Write((byte) VariantType);
    }
    
    public override void ToApexDeferred(BinaryWriter bw)
    {
        bw.Align(Alignment);
        Offset = bw.BaseStream.Position;
        
        if (Count == -1) bw.Write(Values.Length);
        // foreach (var value in Values)
        // {
        //     bw.Write(value);
        // }
    }

    #endregion

    
    #region XmlSerializable

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        xw.WriteNameOrNameHash(NameHash, Name);

        var array = string.Join(",", Values);
        xw.WriteValue(array);
        xw.WriteEndElement();
    }
    
    #endregion
}