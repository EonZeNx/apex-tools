using System.Text;

namespace ApexTools.Core.Hash;

public static class JenkinsL3
{
    public static uint HashJenkins(this string data, int index = 0, uint seed = 0)
    {
        return Hash(Encoding.UTF8.GetBytes(data), index, seed);
    }

    /// <summary>
    /// This is an implementation of Bob Jenkins Lookup3.
    /// </summary>
    public static uint Hash(byte[] data, int index = 0, uint seed = 0)
    {
        uint a = 0xDEADBEEF + (uint) data.Length + seed;
        uint b = a;
        uint c = a;

        // Don't get why just throw error or something, but w/e
        var remainderCount = data.Length % 12;
        if (data.Length != 0 && remainderCount == 0) remainderCount = 12;

        // Blocks of 12, make sure to evenly read 1 block past data.Length
        var remainderOffset = index + data.Length - remainderCount;
        
        // Main shift
        var currentOffset = index;
        while (currentOffset < remainderOffset)
        {
            a += BitConverter.ToUInt32(data, currentOffset);
            b += BitConverter.ToUInt32(data, currentOffset + 4);
            c += BitConverter.ToUInt32(data, currentOffset + 8);

            Mix(ref a, ref b, ref c);

            currentOffset += 12;
        }
        
        // Remainder shift
        switch (remainderCount)
        {
            case 12:
                c += BitConverter.ToUInt32(data, currentOffset + 8);
                goto case 8;

            case 11: c += (uint) data[currentOffset + 10] << 16; goto case 10;
            case 10: c += (uint) data[currentOffset + 9] << 8; goto case 9;
            case 9:  c += (uint) data[currentOffset + 8]; goto case 8;

            case 8:
                b += BitConverter.ToUInt32(data, currentOffset + 4);
                goto case 4;

            case 7: b += (uint) data[currentOffset + 6] << 16; goto case 6;
            case 6: b += (uint) data[currentOffset + 5] << 8; goto case 5;
            case 5: b += (uint) data[currentOffset + 4]; goto case 4;

            case 4:
                a += BitConverter.ToUInt32(data, currentOffset);

                Final(ref a, ref b, ref c);
                break;

            case 3: a += (uint) data[currentOffset + 2] << 16; goto case 2;
            case 2: a += (uint) data[currentOffset + 1] << 8; goto case 1;
            case 1:
                a += data[currentOffset];

                Final(ref a, ref b, ref c);
                break;
        }
        
        // return BitConverter.GetBytes(c);
        // return c;
        return c;
    }
    
    private static void Mix(ref uint a, ref uint b, ref uint c)
    {
        a -= c; a ^= RotateLeft(c, 4); c += b;
        b -= a; b ^= RotateLeft(a,  6); a += c;
        c -= b; c ^= RotateLeft(b,  8); b += a;

        a -= c; a ^= RotateLeft(c, 16); c += b;
        b -= a; b ^= RotateLeft(a, 19); a += c;
        c -= b; c ^= RotateLeft(b,  4); b += a;
    }

    private static void Final(ref uint a, ref uint b, ref uint c)
    {
        c ^= b; c -= RotateLeft(b, 14);
        a ^= c; a -= RotateLeft(c, 11);
        b ^= a; b -= RotateLeft(a, 25);

        c ^= b; c -= RotateLeft(b, 16);
        a ^= c; a -= RotateLeft(c,  4);
        b ^= a; b -= RotateLeft(a, 14);

        c ^= b; c -= RotateLeft(b, 24);
    }

    private static uint RotateLeft(uint operand, int shiftCount)
    {
        shiftCount &= 0x1f;

        return (operand << shiftCount) | (operand >> (32 - shiftCount));
    }
}