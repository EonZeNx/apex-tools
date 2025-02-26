﻿using ApexTools.Core;
using ApexTools.Core.Extensions;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct RtpcV03Header
{
    public EFourCc FourCc;
    public uint Version;
    
    public static int SizeOf() => 4 + 4;

    public RtpcV03Header()
    {
        FourCc = EFourCc.RTPC;
        Version = 3;
    }
}

public static class RtpcV03HeaderExtension
{
    public static RtpcV03Header ReadRtpcV03Header(this BinaryReader br)
    {
        var result = new RtpcV03Header
        {
            FourCc = (EFourCc) br.ReadUInt32(),
            Version = br.ReadUInt32()
        };

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03Header header)
    {
        bw.Write(((uint) header.FourCc).ReverseEndian());
        bw.Write(header.Version);
    }
}