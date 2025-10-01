using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
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

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(SurfaceType);
    }

    protected override Chunk CloneSelf() => new IntersectMesh2Chunk(SurfaceType);
}