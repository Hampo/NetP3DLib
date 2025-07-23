using NetP3DLib.P3D.Enums;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BlackMagicChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Black_Magic;
    
    public override byte[] DataBytes
    {
        get
        {
            return [];
        }
    }
    public override uint DataLength => 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for ChunkLoader.")]
    public BlackMagicChunk(BinaryReader br) : base(ChunkID)
    { }

    public BlackMagicChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    { }

    internal override Chunk CloneSelf() => new BlackMagicChunk();
}