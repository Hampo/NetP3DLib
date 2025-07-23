using NetP3DLib.P3D.Enums;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionWallChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Wall;
    
    public override byte[] DataBytes
    {
        get
        {
            return [];
        }
    }
    public override uint DataLength => 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for ChunkLoader.")]
    public CollisionWallChunk(BinaryReader br) : base(ChunkID)
    { }

    public CollisionWallChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    { }

    internal override Chunk CloneSelf() => new CollisionWallChunk();
}