using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

namespace EonZeNx.ApexFormats.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="MemberV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>Name</term>
///         <description><see cref="ulong"/></description>
///     </item>
///     <item>
///         <term>Type hash</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Alignment</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Bit offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Flags</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Default value</term>
///         <description><see cref="ulong"/></description>
///     </item>
/// </list>
/// </summary>
public class MemberV04 : IApexSerializable
{
    // Create properties from summary
    public ulong Name { get; set; }
    public uint TypeHash { get; set; }
    public uint Alignment { get; set; }
    public uint Offset { get; set; }
    public uint BitOffset { get; set; }
    public uint Flags { get; set; }
    public ulong DefaultValue { get; set; }


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        Name = br.ReadUInt64();
        TypeHash = br.ReadUInt32();
        Alignment = br.ReadUInt32();
        Offset = br.ReadUInt32();
        BitOffset = br.ReadUInt32();
        Flags = br.ReadUInt32();
        DefaultValue = br.ReadUInt64();
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}