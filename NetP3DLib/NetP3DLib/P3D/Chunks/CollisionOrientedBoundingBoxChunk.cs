using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Oriented_Bounding_Box)]
public class CollisionOrientedBoundingBoxChunk : Chunk
{
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

    public CollisionOrientedBoundingBoxChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Oriented_Bounding_Box)
    {
        HalfExtents = br.ReadVector3();
    }

    public CollisionOrientedBoundingBoxChunk(Vector3 halfExtents) : base((uint)ChunkIdentifier.Collision_Oriented_Bounding_Box)
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