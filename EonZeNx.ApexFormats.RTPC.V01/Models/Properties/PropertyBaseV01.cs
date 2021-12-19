using System.Xml;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

public abstract class PropertyBaseV01 : XmlSerializable, IApexSerializable
{
    public override string XmlName => "PropertyBase";
    public static int HeaderSize => 4 + 4 + 1;
    public abstract EVariantType VariantType { get; }
    public abstract bool Primitive { get; }
    
    public int NameHash { get; set; }
    public string HexNameHash => ByteUtils.ToHex(NameHash);
    public byte[] RawData { get; set; } = Array.Empty<byte>();
    
    
    private string _name = string.Empty;
    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name)) _name = HashUtils.Lookup(NameHash);
            return _name;
        }
    }


    #region ApexSerializable

    public abstract void FromApex(BinaryReader br);
    public abstract void ToApex(BinaryWriter bw);
    
    #endregion


    #region XmlSerializable

    public abstract override void FromXml(XmlReader xr);
    public abstract override void ToXml(XmlWriter xw);

    #endregion
}