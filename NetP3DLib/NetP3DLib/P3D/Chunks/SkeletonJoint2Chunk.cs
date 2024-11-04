using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Skeleton_Joint_2)]
public class SkeletonJoint2Chunk : NamedChunk
{
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) * 16;

    public SkeletonJoint2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Skeleton_Joint_2)
    {
        Name = br.ReadP3DString();
        Parent = br.ReadUInt32();
        RestPose = br.ReadMatrix4x4();
    }

    public SkeletonJoint2Chunk(string name, uint parent, int dof, int freeAxis, int primaryAxis, int secondaryAxis, int twistAxis, Matrix4x4 restPose) : base((uint)ChunkIdentifier.Skeleton_Joint_2)
    {
        Name = name;
        Parent = parent;
        RestPose = restPose;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Parent);
        bw.Write(RestPose);
    }
}