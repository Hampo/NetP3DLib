using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IntersectMesh2Chunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Intersect_Mesh_2;
    
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

    public IntersectMesh2Chunk(BinaryReader br) : base(ChunkID)
    {
        SurfaceType = br.ReadUInt32();
    }

    public IntersectMesh2Chunk(uint surfaceType) : base(ChunkID)
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