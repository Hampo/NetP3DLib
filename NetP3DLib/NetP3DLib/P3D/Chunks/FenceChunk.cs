using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FenceChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Fence;
    
    public override byte[] DataBytes
    {
        get
        {
            return [];
        }
    }
    public override uint DataLength => 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for ChunkLoader.")]
    public FenceChunk(BinaryReader br) : base(ChunkID)
    { }

    public FenceChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    { }

    internal override Chunk CloneSelf() => new FenceChunk();
}