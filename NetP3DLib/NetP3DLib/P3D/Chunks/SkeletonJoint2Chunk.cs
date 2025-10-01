using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJoint2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint_2;
    
    public uint Parent { get; set; }
    public Matrix4x4 RestPose { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Parent));
            data.AddRange(BinaryExtensions.GetBytes(RestPose));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) * 16;

    public SkeletonJoint2Chunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Parent = br.ReadUInt32();
        RestPose = br.ReadMatrix4x4();
    }

    public SkeletonJoint2Chunk(string name, uint parent, Matrix4x4 restPose) : base(ChunkID)
    {
        Name = name;
        Parent = parent;
        RestPose = restPose;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Parent);
        bw.Write(RestPose);
    }

    protected override Chunk CloneSelf() => new SkeletonJoint2Chunk(Name, Parent, RestPose);
}