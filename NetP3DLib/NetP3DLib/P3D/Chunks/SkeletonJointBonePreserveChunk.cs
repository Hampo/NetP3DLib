using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJointBonePreserveChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint_Bone_Preserve;
    
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

    public SkeletonJointBonePreserveChunk(BinaryReader br) : base(ChunkID)
    {
        PreserveBoneLengths = br.ReadUInt32();
    }

    public SkeletonJointBonePreserveChunk(uint preserveBoneLengths) : base(ChunkID)
    {
        PreserveBoneLengths = preserveBoneLengths;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(PreserveBoneLengths);
    }

    internal override Chunk CloneSelf() => new SkeletonJointBonePreserveChunk(PreserveBoneLengths);
}