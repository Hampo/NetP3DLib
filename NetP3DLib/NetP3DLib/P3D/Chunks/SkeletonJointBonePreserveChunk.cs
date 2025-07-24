using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJointBonePreserveChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint_Bone_Preserve;

    private uint preserveBoneLengths;
    public bool PreserveBoneLengths
    {
        get => preserveBoneLengths != 0;
        set => preserveBoneLengths = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(preserveBoneLengths));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public SkeletonJointBonePreserveChunk(BinaryReader br) : base(ChunkID)
    {
        preserveBoneLengths = br.ReadUInt32();
    }

    public SkeletonJointBonePreserveChunk(bool preserveBoneLengths) : base(ChunkID)
    {
        PreserveBoneLengths = preserveBoneLengths;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(preserveBoneLengths);
    }

    internal override Chunk CloneSelf() => new SkeletonJointBonePreserveChunk(PreserveBoneLengths);
}