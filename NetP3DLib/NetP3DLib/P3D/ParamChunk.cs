using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;

namespace NetP3DLib.P3D;

public abstract class ParamChunk : Chunk
{
    public string Param { get; set; }

    public ParamChunk(uint ID) : base(ID) { }

    public ParamChunk(ChunkIdentifier ID) : base(ID) { }

    public override void Validate()
    {
        if (!Param.IsValidFourCC())
            throw new InvalidFourCCException(nameof(Param), Param);

        base.Validate();
    }

    public override string ToString() => $"\"{Param}\" ({GetChunkType(this)} (0x{ID:X}))";
}
