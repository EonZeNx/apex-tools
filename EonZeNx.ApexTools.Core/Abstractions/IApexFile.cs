namespace EonZeNx.ApexTools.Core.Abstractions;

public interface IApexFile
{
    EFourCc FourCc { get; }
    uint Version { get; }
}