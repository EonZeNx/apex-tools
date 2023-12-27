namespace ApexTools.Core.Abstractions;

public interface IApexFile
{
    EFourCc FourCc { get; }
    uint Version { get; }
}