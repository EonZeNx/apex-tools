using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="FileHeaderV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>Instance count</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>First instance offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Type count</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>First type offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>String hash count</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>First string hash offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>String count</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>First string offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>File size</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Unknown</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Flags</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Included libraries</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Unknown2</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Unknown3</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Comment</term>
///         <description><see cref="string"/></description>
///     </item>
/// </list>
/// </summary>
public class FileHeaderV04 : IApexSerializable, ICustomFileSerializable
{
    public uint InstanceCount { get; set; }
    public uint FirstInstanceOffset { get; set; }
    public uint TypeDefCount { get; set; }
    public uint FirstTypeDefOffset { get; set; }
    public uint StringHashCount { get; set; }
    public uint FirstStringHashOffset { get; set; }
    public uint StringCount { get; set; }
    public uint FirstStringOffset { get; set; }
    public uint FileSize { get; set; }
    public uint Unknown { get; set; }
    public uint Flags { get; set; }
    public uint IncludedLibraries { get; set; }
    public uint Unknown2 { get; set; }
    public uint Unknown3 { get; set; }
    public string Comment { get; set; } = string.Empty;


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        InstanceCount = br.ReadUInt32();
        FirstInstanceOffset = br.ReadUInt32();
        TypeDefCount = br.ReadUInt32();
        FirstTypeDefOffset = br.ReadUInt32();
        StringHashCount = br.ReadUInt32();
        FirstStringHashOffset = br.ReadUInt32();
        StringCount = br.ReadUInt32();
        FirstStringOffset = br.ReadUInt32();
        FileSize = br.ReadUInt32();
        Unknown = br.ReadUInt32();
        Flags = br.ReadUInt32();
        IncludedLibraries = br.ReadUInt32();
        Unknown2 = br.ReadUInt32();
        Unknown3 = br.ReadUInt32();
        Comment = br.ReadStringZ();
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    
    #region XmlSerializable

    public void FromCustomFile(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public void ToCustomFile(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}