using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionSphereChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Sphere;

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

    public CollisionSphereChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Radius = br.ReadSingle();
    }

    public CollisionSphereChunk(float radius) : base(ChunkID)
    {
        Radius = radius;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Radius);
    }

    protected override Chunk CloneSelf() => new CollisionSphereChunk(Radius);
}