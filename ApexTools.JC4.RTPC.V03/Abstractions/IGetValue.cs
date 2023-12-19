namespace ApexTools.JC4.RTPC.V03.Abstractions;

public interface IGetValue<out T>
{
    T GetValue();
}