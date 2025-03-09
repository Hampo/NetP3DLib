using System.IO;
using System.Text;

namespace NetP3DLib.P3D;

public abstract class NamedChunk : Chunk
{
    public string Name { get; set; }

    public NamedChunk(uint ID) : base(ID) { }

    public NamedChunk(ChunkIdentifier ID) : base(ID) { }

    public override void Validate()
    {
        if (Name == null)
            throw new InvalidDataException($"{nameof(Name)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Name).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Name)} is 255 bytes.");

        base.Validate();
    }

    public override string ToString() => $"\"{Name}\" ({GetChunkType(this)} (0x{ID:X}))";
}
