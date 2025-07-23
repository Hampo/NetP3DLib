using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJointChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint;
    
    public uint Parent { get; set; }
    public int DOF { get; set; }
    public int FreeAxis { get; set; }
    public int PrimaryAxis { get; set; }
    public int SecondaryAxis { get; set; }
    public int TwistAxis { get; set; }
    public Matrix4x4 RestPose { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Parent));
            data.AddRange(BitConverter.GetBytes(DOF));
            data.AddRange(BitConverter.GetBytes(FreeAxis));
            data.AddRange(BitConverter.GetBytes(PrimaryAxis));
            data.AddRange(BitConverter.GetBytes(SecondaryAxis));
            data.AddRange(BitConverter.GetBytes(TwistAxis));
            data.AddRange(BinaryExtensions.GetBytes(RestPose));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(float) * 16;

    public SkeletonJointChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Parent = br.ReadUInt32();
        DOF = br.ReadInt32();
        FreeAxis = br.ReadInt32();
        PrimaryAxis = br.ReadInt32();
        SecondaryAxis = br.ReadInt32();
        TwistAxis = br.ReadInt32();
        RestPose = br.ReadMatrix4x4();
    }

    public SkeletonJointChunk(string name, uint parent, int dof, int freeAxis, int primaryAxis, int secondaryAxis, int twistAxis, Matrix4x4 restPose) : base(ChunkID)
    {
        Name = name;
        Parent = parent;
        DOF = dof;
        FreeAxis = freeAxis;
        PrimaryAxis = primaryAxis;
        SecondaryAxis = secondaryAxis;
        TwistAxis = twistAxis;
        RestPose = restPose;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Parent);
        bw.Write(DOF);
        bw.Write(FreeAxis);
        bw.Write(PrimaryAxis);
        bw.Write(SecondaryAxis);
        bw.Write(TwistAxis);
        bw.Write(RestPose);
    }

    internal override Chunk CloneSelf() => new SkeletonJointChunk(Name, Parent, DOF, FreeAxis, PrimaryAxis, SecondaryAxis, TwistAxis, RestPose);
}