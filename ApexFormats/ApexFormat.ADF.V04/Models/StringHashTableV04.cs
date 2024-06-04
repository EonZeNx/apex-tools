using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Extensions;

namespace ApexFormat.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="StringHashTableV04"/></b>.
/// <br/> Wrapper for <see cref="ulong">String hash</see> to <see cref="string"/> dictionary
/// </summary>
public class StringHashTableV04 : IApexSerializable
{
    public long Offset { get; set; }
    public uint Count { get; set; }
    public Dictionary<ulong, string> StringHashes { get; set; } = new();

    
    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        var originalPosition = br.Position();
        br.BaseStream.Seek(Offset, SeekOrigin.Begin);

        for (var i = 0; i < Count; i++)
        {
            var value = br.ReadStringZ();
            StringHashes.Add(br.ReadUInt64(), value);
        }

        br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}