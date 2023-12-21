using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.Models;

/// <summary>
/// Format:<br/>
/// Name hash - <see cref="int"/><br/>
/// RawData - <see cref="byte"/>[]<br/>
/// VariantType - <see cref="EVariantType"/>
/// </summary>
public class PropertyHeaderV03 : IBinarySize, IFromApex, IToApex
{
    public uint NameHash = 0;
    public byte[] RawData = new byte[4];
    public EVariantType VariantType = EVariantType.Unassigned;
    
    public string HexNameHash => ByteUtils.ToHex(NameHash, true);
    private string _name = string.Empty;
    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name)) _name = HashUtils.Lookup(NameHash);
            return _name;
        }
    }

    public static int BinarySize => 4 + 4 + 1;

    public void FromApex(BinaryReader br)
    {
        NameHash = br.ReadUInt32();
        RawData = br.ReadBytes(4);
        VariantType = (EVariantType)br.ReadByte();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(RawData);
        bw.Write((byte) VariantType);
    }
}