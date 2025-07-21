using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BoundingSphereChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Bounding_Sphere;
    
    public Vector3 Centre { get; set; }
    public float Radius { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Centre));
            data.AddRange(BitConverter.GetBytes(Radius));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3 + sizeof(float);

    public BoundingSphereChunk(BinaryReader br) : base(ChunkID)
    {
        Centre = br.ReadVector3();
        Radius = br.ReadSingle();
    }

    public BoundingSphereChunk(Vector3 centre, float radius) : base(ChunkID)
    {
        Centre = centre;
        Radius = radius;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Centre);
        bw.Write(Radius);
    }
}