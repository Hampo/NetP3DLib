using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System.Collections.Generic;

namespace NetP3DLib.P3D;

#pragma warning disable CS8618
public abstract class NamedChunk : Chunk
{
    internal P3DString _name;
    public string Name
    {
        get => _name?.Value ?? string.Empty;
        set => _name.Value = value;
    }

    protected NamedChunk(uint ID) : base(ID)
    {
    }

    protected NamedChunk(ChunkIdentifier ID) : base(ID)
    {
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Name.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Name), Name);
    }

    public override string ToString() => $"\"{Name}\" ({GetChunkType(this)} (0x{ID:X}))";
}
#pragma warning restore CS8618