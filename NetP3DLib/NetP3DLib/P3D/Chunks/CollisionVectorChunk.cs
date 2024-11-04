using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVectorChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Vector;
    
    public Vector3 Vector { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Vector));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public CollisionVectorChunk(BinaryReader br) : base(ChunkID)
    {
        Vector = br.ReadVector3();
    }

    public CollisionVectorChunk(Vector3 vector) : base(ChunkID)
    {
        Vector = vector;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Vector);
    }
}