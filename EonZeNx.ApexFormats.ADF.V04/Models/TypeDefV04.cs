using System.ComponentModel;
using System.Xml;
using EonZeNx.ApexFormats.ADF.V04.Abstractions;
using EonZeNx.ApexFormats.ADF.V04.Models.Types;
using EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants;
using EonZeNx.ApexFormats.ADF.V04.Utils;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

namespace EonZeNx.ApexFormats.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="TypeDefV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>Variant type</term>
///         <description><see cref="EVariantType"/></description>
///     </item>
///     <item>
///         <term>Size</term>
///         <description><see cref="EScalarType"/></description>
///     </item>
///     <item>
///         <term>Alignment</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Type hash</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Name</term>
///         <description><see cref="ulong"/></description>
///     </item>
///     <item>
///         <term>Flags</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>SubType hash</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Array size || Bit count</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Member count || Data align</term>
///         <description><see cref="uint"/></description>
///     </item>
/// </list>
/// </summary>
public class TypeDefV04 : IApexSerializable, ITypeDefV04
{
    public EVariantType VariantType { get; set; }
    public uint Size { get; set; }
    public uint Alignment { get; set; }
    public uint TypeHash { get; set; }
    public ulong NameHash { get; set; }
    public ushort Flags { get; set; }
    public EScalarType ScalarType { get; set; }
    public uint SubTypeHash { get; set; }
    public uint ArraySizeOrBitCount { get; set; }


    #region ApexSerializable

    public virtual void FromApex(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public virtual void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    
    #region LoadDefinition
    
    public virtual void LoadDefinitionFromApex(BinaryReader br)
    {
        VariantType = (EVariantType) br.ReadUInt32();
        Size = br.ReadUInt32();
        Alignment = br.ReadUInt32();
        TypeHash = br.ReadUInt32();
        NameHash = br.ReadUInt64();
        Flags = br.ReadUInt16();
        ScalarType = (EScalarType) br.ReadUInt16();
        SubTypeHash = br.ReadUInt32();
        ArraySizeOrBitCount = br.ReadUInt32();
    }

    public virtual void LoadDefinitionFromXml(XmlWriter xw)
    {
        throw new NotImplementedException();
    }
    
    #endregion
}