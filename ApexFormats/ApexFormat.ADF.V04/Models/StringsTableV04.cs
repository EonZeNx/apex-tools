using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Utils;

namespace ApexFormat.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="StringsTableV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>Length of names</term>
///         <description><see cref="byte"/>[]</description>
///     </item>
///     <item>
///         <term>Names</term>
///         <description><see cref="string"/>[]</description>
///     </item>
/// </list>
/// </summary>
public class StringsTableV04 : IApexSerializable
{
    public long Offset { get; set; }
    public uint Count { get; set; }
    public (byte, string)[] Names { get; set; } = Array.Empty<(byte, string)>();

    
    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        var originalPosition = br.Position();
        br.BaseStream.Seek(Offset, SeekOrigin.Begin);

        var lengthOfNames = br.ReadBytes((int) Count);
        
        Names = new (byte, string)[Count];
        for (var i = 0; i < Count; i++)
        {
            Names[i] = (lengthOfNames[i], br.ReadStringZ());
        }

        br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}