namespace ApexTools.JC4.RTPC.V03.Exceptions;

public class RtpcContainerException : Exception
{
    public RtpcContainerException()
    {
    }

    public RtpcContainerException(string? message) : base(message)
    {
    }

    public RtpcContainerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}