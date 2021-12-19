﻿using System.Buffers.Binary;

namespace EonZeNx.ApexTools.Core.Utils;

public static class Extensions
{
    public static long Position(this BinaryReader br)
    {
        return br.BaseStream.Position;
    }
    
    public static long Position(this BinaryWriter bw)
    {
        return bw.BaseStream.Position;
    }
    
    
    public static uint ReadBigUInt32(this BinaryReader br)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(br.ReadBytes(4));
    }

    
    public static void Align(this BinaryReader br, uint align)
    {
        br.BaseStream.Seek(ByteUtils.Align(br.Position(), align), SeekOrigin.Begin);
    }
        
    public static void Align(this BinaryWriter bw, long align, byte fill = 0x50)
    {
        var pos = bw.Position();
        var alignment = ByteUtils.Align(pos, align);
        for (var i = 0; i < alignment - pos; i++)
        {
            bw.Write(fill);
        }
    }
        
    public static long Align(this MemoryStream ms, long offset, long align)
    {
        var preAlign = ByteUtils.Align(offset, align);
        var alignment = preAlign - offset;
        for (var i = 0; i < alignment; i++)
        {
            ms.WriteByte(0x50);
        }

        return preAlign;
    }
}