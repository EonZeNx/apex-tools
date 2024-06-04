using ApexTools.Core.Interfaces;

namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public class RtpcV03InlineHeader : IFromApex, IToApex
{
    public static byte Version01 => 0x01;
    public static ushort Version02 => 0x04;
    
    public ushort ContainerCount;

    public void FromApex(BinaryReader br)
    {
        if (br.ReadByte() != Version01) throw new FileLoadException();
        if (br.ReadUInt16() != Version02) throw new FileLoadException();
        
        ContainerCount = br.ReadUInt16();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(Version01);
        bw.Write(Version02);
        
        bw.Write(ContainerCount);
    }
}