using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Vector)]
public class CollisionVectorChunk : Chunk
{
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

    public CollisionVectorChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Vector)
    {
        Vector = br.ReadVector3();
    }

    public CollisionVectorChunk(Vector3 vector) : base((uint)ChunkIdentifier.Collision_Vector)
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