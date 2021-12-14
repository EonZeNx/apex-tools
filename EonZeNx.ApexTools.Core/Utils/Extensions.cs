using System.Buffers.Binary;

namespace EonZeNx.ApexTools.Core.Utils;

public static class Extensions
{
    public static long Position(this BinaryReader br)
    {
        return br.BaseStream.Position;
    }
    
    public static uint ReadBigUInt32(this BinaryReader br)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(br.ReadBytes(4));
    }
    
    public static long Position(this BinaryWriter bw)
    {
        return bw.BaseStream.Position;
    }
}