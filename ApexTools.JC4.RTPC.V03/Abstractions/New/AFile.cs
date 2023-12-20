namespace ApexTools.JC4.RTPC.V03.Abstractions.New;

public abstract class AFile
{
    public abstract string Extension { get; set; }
    public abstract FileHeader Header { get; }
}
