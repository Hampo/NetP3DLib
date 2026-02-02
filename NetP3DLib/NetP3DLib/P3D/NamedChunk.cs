using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;

namespace NetP3DLib.P3D;

public abstract class NamedChunk : Chunk
{
    public string Name { get; set; } = string.Empty;

    public NamedChunk(uint ID) : base(ID) { }

    public NamedChunk(ChunkIdentifier ID) : base(ID) { }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!Name.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Name), Name);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    public override string ToString() => $"\"{Name}\" ({GetChunkType(this)} (0x{ID:X}))";
}
