using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Extensions;

namespace ApexFormat.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="TypeDefTableV04"/></b>.
/// <br/> Wrapper for <see cref="TypeDefV04"/> dictionary
/// </summary>
public class TypeDefTableV04 : IApexSerializable
{
    public long Offset { get; set; }
    public uint Count { get; set; }
    public Dictionary<uint, TypeDefV04> TypeDefs { get; set; } = new();

    
    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        var originalPosition = br.Position();
        br.BaseStream.Seek(Offset, SeekOrigin.Begin);

        for (var i = 0; i < Count; i++)
        {
            var typeDef = new TypeDefV04();
            typeDef.FromApex(br);
            TypeDefs.Add(typeDef.TypeHash, typeDef);
        }

        br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}