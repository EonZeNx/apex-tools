using System.Text;

namespace ApexTools.JC4.RTPC.V03;

public class RtpcBinaryReader : BinaryReader
{
    public RtpcBinaryReader(Stream input) : base(input)
    {}

    public RtpcBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
    {}

    public RtpcBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
    {}
}