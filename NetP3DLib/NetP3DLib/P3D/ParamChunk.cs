using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System.Collections.Generic;

namespace NetP3DLib.P3D;

#pragma warning disable CS8618
public abstract class ParamChunk : Chunk
{
    internal FourCC _param;
    [MaxLength(4)]
    public string Param
    {
        get => _param.Value ?? string.Empty;
        set => _param.Value = value;
    }

    public ParamChunk(uint ID) : base(ID)
    {
    }

    public ParamChunk(ChunkIdentifier ID) : base(ID)
    {
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Param.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(Param), Param);
    }

    public override string ToString() => $"\"{Param}\" ({GetChunkType(this)} (0x{ID:X}))";
}
#pragma warning restore CS8618