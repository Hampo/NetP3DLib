using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;

namespace NetP3DLib.P3D;

public abstract class NamedChunk : Chunk
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
                return;

            _name = value;
            RecalculateSize();
        }
    }

    public NamedChunk(uint ID) : base(ID) { }

    public NamedChunk(ChunkIdentifier ID) : base(ID) { }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Name.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Name), Name);
    }

    public override string ToString() => $"\"{Name}\" ({GetChunkType(this)} (0x{ID:X}))";
}
