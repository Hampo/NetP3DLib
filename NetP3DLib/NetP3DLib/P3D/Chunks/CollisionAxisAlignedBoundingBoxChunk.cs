using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionAxisAlignedBoundingBoxChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Axis_Aligned_Bounding_Box;
    
    public uint Nothing { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Nothing));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public CollisionAxisAlignedBoundingBoxChunk(BinaryReader br) : base(ChunkID)
    {
        Nothing = br.ReadUInt32();
    }

    public CollisionAxisAlignedBoundingBoxChunk() : base(ChunkID)
    {
        Nothing = 0;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Nothing);
    }
}