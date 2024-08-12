using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Axis_Aligned_Bounding_Box)]
public class CollisionAxisAlignedBoundingBoxChunk : Chunk
{
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

    public CollisionAxisAlignedBoundingBoxChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Axis_Aligned_Bounding_Box)
    {
        Nothing = br.ReadUInt32();
    }

    public CollisionAxisAlignedBoundingBoxChunk() : base((uint)ChunkIdentifier.Collision_Axis_Aligned_Bounding_Box)
    {
        Nothing = 0;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Nothing);
    }
}