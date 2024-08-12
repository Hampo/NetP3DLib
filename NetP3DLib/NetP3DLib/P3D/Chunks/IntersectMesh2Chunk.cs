using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Intersect_Mesh_2)]
public class IntersectMesh2Chunk : Chunk
{
    public uint SurfaceType { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(SurfaceType));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public IntersectMesh2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Intersect_Mesh_2)
    {
        SurfaceType = br.ReadUInt32();
    }

    public IntersectMesh2Chunk(uint surfaceType) : base((uint)ChunkIdentifier.Intersect_Mesh_2)
    {
        SurfaceType = surfaceType;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(SurfaceType);
    }
}