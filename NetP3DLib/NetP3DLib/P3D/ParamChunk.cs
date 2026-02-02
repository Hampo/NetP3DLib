using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;

namespace NetP3DLib.P3D;

public abstract class ParamChunk : Chunk
{
    [MaxLength(4)]
    public string Param { get; set; } = string.Empty;

    public ParamChunk(uint ID) : base(ID) { }

    public ParamChunk(ChunkIdentifier ID) : base(ID) { }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!Param.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(Param), Param);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    public override string ToString() => $"\"{Param}\" ({GetChunkType(this)} (0x{ID:X}))";
}
