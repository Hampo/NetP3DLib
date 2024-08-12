using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Skeleton_Joint_Bone_Preserve)]
public class SkeletonJointBonePreserveChunk : Chunk
{
    public uint PreserveBoneLengths { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(PreserveBoneLengths));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public SkeletonJointBonePreserveChunk(BinaryReader br) : base((uint)ChunkIdentifier.Skeleton_Joint_Bone_Preserve)
    {
        PreserveBoneLengths = br.ReadUInt32();
    }

    public SkeletonJointBonePreserveChunk(uint preserveBoneLengths) : base((uint)ChunkIdentifier.Skeleton_Joint_Bone_Preserve)
    {
        PreserveBoneLengths = preserveBoneLengths;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(PreserveBoneLengths);
    }
}