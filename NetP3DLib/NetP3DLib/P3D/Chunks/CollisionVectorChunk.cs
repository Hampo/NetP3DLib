using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.Numerics;
using static NetP3DLib.P3D.Chunks.InstParticleSystemChunk;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVectorChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Vector;

    public Vector3 Vector { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Vector));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public CollisionVectorChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Vector = br.ReadVector3();
    }

    public CollisionVectorChunk(Vector3 vector) : base(ChunkID)
    {
        Vector = vector;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Vector);
    }

    protected override Chunk CloneSelf() => new CollisionVectorChunk(Vector);

    public override string ToString()
    {
        return ParentChunk switch
        {
            CollisionSphereChunk => IndexInParent switch
            {
                0 => $"Centre ({GetChunkType(this)} (0x{ID:X}))",
                _ => base.ToString()
            },
            CollisionCylinderChunk => IndexInParent switch
            {
                0 => $"Centre ({GetChunkType(this)} (0x{ID:X}))",
                1 => $"Orientation ({GetChunkType(this)} (0x{ID:X}))",
                _ => base.ToString()
            },
            CollisionOrientedBoundingBoxChunk => IndexInParent switch
            {
                0 => $"Centre ({GetChunkType(this)} (0x{ID:X}))",
                1 => $"Rotation Matrix X ({GetChunkType(this)} (0x{ID:X}))",
                2 => $"Rotation Matrix Y ({GetChunkType(this)} (0x{ID:X}))",
                3 => $"Rotation Matrix Z ({GetChunkType(this)} (0x{ID:X}))",
                _ => base.ToString()
            },
            _ => base.ToString(),
        };
    }
}