using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionOrientedBoundingBoxChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Oriented_Bounding_Box;
    
    public Vector3 HalfExtents { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(HalfExtents));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public CollisionOrientedBoundingBoxChunk(BinaryReader br) : base(ChunkID)
    {
        HalfExtents = br.ReadVector3();
    }

    public CollisionOrientedBoundingBoxChunk(Vector3 halfExtents) : base(ChunkID)
    {
        HalfExtents = halfExtents;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(HalfExtents);
    }
}