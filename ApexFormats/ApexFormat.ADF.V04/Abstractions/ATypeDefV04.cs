﻿using ApexFormat.ADF.V04.Models.Types;
using ApexTools.Core.Abstractions.CombinedSerializable;

namespace ApexFormat.ADF.V04.Abstractions;

public abstract class ATypeDefV04 : IApexSerializable
{
    public abstract EVariantType VariantType { get; }


    #region ApexSerializable
    public abstract void FromApex(BinaryReader br);
    public abstract void ToApex(BinaryWriter bw);
    #endregion
}