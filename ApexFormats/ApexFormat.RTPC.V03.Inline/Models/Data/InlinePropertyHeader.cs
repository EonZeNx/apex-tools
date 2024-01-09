using ApexTools.Core.Interfaces;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public class InlinePropertyHeader : IFromApex
{
    public uint NameHash { get; private set; }
    public EVariantType VariantType { get; private set; }
    
    public long Offset { get; private set; }
    
    public void FromApex(BinaryReader br)
    {
        Offset = br.Position();
        
        NameHash = br.ReadUInt32();
        VariantType = (EVariantType) br.ReadByte();
    }
}