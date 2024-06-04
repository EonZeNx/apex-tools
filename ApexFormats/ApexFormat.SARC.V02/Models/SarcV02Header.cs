using ApexTools.Core;
using ApexTools.Core.Extensions;

namespace ApexFormat.SARC.V02.Models;

public static class SarcV02HeaderConstants
{
    public const uint MagicLength = 0x04;
    public const EFourCc Magic = EFourCc.SARC;
    public const uint Version = 0x02;
}

/// <summary>
/// Structure:
/// <br/>MagicLength - <see cref="uint"/>
/// <br/>Magic - <see cref="EFourCc"/>
/// <br/>Version - <see cref="uint"/>
/// <br/>Size - <see cref="uint"/>
/// </summary>
public class SarcV02Header
{
    public uint MagicLength = SarcV02HeaderConstants.MagicLength;
    public EFourCc Magic = SarcV02HeaderConstants.Magic;
    public uint Version = SarcV02HeaderConstants.Version;
    public uint DataOffset;

    public static uint SizeOf() => 4 + 4 + 4 + 4;
}

public static class SarcV02HeaderExtensions
{
    public static SarcV02Header ReadSarcV02Header(this BinaryReader br)
    {
        var result = new SarcV02Header
        {
            MagicLength = br.ReadUInt32(),
            Magic = (EFourCc) br.ReadUInt32().ReverseEndian(),
            Version = br.ReadUInt32(),
            DataOffset = br.ReadUInt32()
        };

        if (result.MagicLength != SarcV02HeaderConstants.MagicLength)
        {
            throw new FileLoadException($"{nameof(result.MagicLength)} is {result.MagicLength}, expected {SarcV02HeaderConstants.MagicLength}");
        }

        if (result.Magic != SarcV02HeaderConstants.Magic)
        {
            throw new FileLoadException($"{nameof(result.Magic)} is {result.Magic}, expected {SarcV02HeaderConstants.Magic}");
        }

        if (result.Version != SarcV02HeaderConstants.Version)
        {
            throw new FileLoadException($"{nameof(result.Version)} is {result.Version}, expected {SarcV02HeaderConstants.Version}");
        }

        return result;
    }
    public static void Write(this BinaryWriter bw, SarcV02Header header)
    {
        bw.Write(header.MagicLength);
        bw.Write(((uint) header.Magic).ReverseEndian());
        bw.Write(header.Version);
        bw.Write(header.DataOffset);
    }
}