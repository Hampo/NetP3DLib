using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJointMirrorMapChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint_Mirror_Map;
    
    public uint MappedJointIndex { get; set; }
    public float XAxisMap { get; set; }
    public float YAxisMap { get; set; }
    public float ZAxisMap { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(MappedJointIndex));
            data.AddRange(BitConverter.GetBytes(XAxisMap));
            data.AddRange(BitConverter.GetBytes(YAxisMap));
            data.AddRange(BitConverter.GetBytes(ZAxisMap));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float);

    public SkeletonJointMirrorMapChunk(BinaryReader br) : base(ChunkID)
    {
        MappedJointIndex = br.ReadUInt32();
        XAxisMap = br.ReadSingle();
        YAxisMap = br.ReadSingle();
        ZAxisMap = br.ReadSingle();
    }

    public SkeletonJointMirrorMapChunk(uint mappedJointIndex, float xAxisMap, float yAxisMap, float zAxisMap) : base(ChunkID)
    {
        MappedJointIndex = mappedJointIndex;
        XAxisMap = xAxisMap;
        YAxisMap = yAxisMap;
        ZAxisMap = zAxisMap;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(MappedJointIndex);
        bw.Write(XAxisMap);
        bw.Write(YAxisMap);
        bw.Write(ZAxisMap);
    }

    protected override Chunk CloneSelf() => new SkeletonJointMirrorMapChunk(MappedJointIndex, XAxisMap, YAxisMap, ZAxisMap);
}