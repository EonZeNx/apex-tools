﻿using System.Xml;
using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Models.Properties;

public class FloatArray : BasePropertyV01
{
    public override string XmlName => "FloatArray";
    protected override EVariantType VariantType => EVariantType.Vec2;

    protected static int Num { get; set; } = 2;
    public float[] Value { get; set; } = Array.Empty<float>();
    
    
    public FloatArray() { }
    public FloatArray(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01) { }
    
    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = new float[Num];
        for (var i = 0; i < Num; i++)
        {
            Value[i] = br.ReadSingle();
        }
    }

    public override void ToApex(BinaryWriter bw)
    {
        base.ToApex(bw);
        
        foreach (var val in Value)
        {
            bw.Write(val);
        }
    }
    
    #endregion

    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var floatString = xr.ReadElementContentAsString();
        var floats = floatString.Split(", ");
        Value = Array.ConvertAll(floats, float.Parse);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        xw.WriteAttributeString("Offset", ByteUtils.ToHex((uint) Offset));
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

        var array = string.Join(", ", Value);
        xw.WriteValue(array);
        xw.WriteEndElement();
    }

    #endregion
}