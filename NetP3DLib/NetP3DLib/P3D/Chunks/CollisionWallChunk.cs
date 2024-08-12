using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Wall)]
public class CollisionWallChunk : Chunk
{
    public override byte[] DataBytes
    {
        get
        {
            return [];
        }
    }
    public override uint DataLength => 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for ChunkLoader.")]
    public CollisionWallChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Wall)
    { }

    public CollisionWallChunk() : base((uint)ChunkIdentifier.Collision_Wall)
    { }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    { }
}