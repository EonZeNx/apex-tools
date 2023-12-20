using EonZeNx.ApexTools.Core;

namespace ApexTools.JC4.RTPC.V03.Abstractions.New;

public interface IFileHeaderStatic
{
    static abstract EFourCc FourCc { get; }
    static abstract int Version { get; }
}