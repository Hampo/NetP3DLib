using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Sphere)]
public class CollisionSphereChunk : Chunk
{
    public float Radius { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Radius));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float);

    public CollisionSphereChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Sphere)
    {
        Radius = br.ReadSingle();
    }

    public CollisionSphereChunk(float radius) : base((uint)ChunkIdentifier.Collision_Sphere)
    {
        Radius = radius;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Radius);
    }
}