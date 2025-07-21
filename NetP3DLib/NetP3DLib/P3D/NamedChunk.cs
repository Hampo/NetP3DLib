using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;

namespace NetP3DLib.P3D;

public abstract class NamedChunk : Chunk
{
    public string Name { get; set; }

    public NamedChunk(uint ID) : base(ID) { }

    public NamedChunk(ChunkIdentifier ID) : base(ID) { }

    public override void Validate()
    {
        if (!Name.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Name), Name);

        base.Validate();
    }

    public override string ToString() => $"\"{Name}\" ({GetChunkType(this)} (0x{ID:X}))";
}
