using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhysicsJointChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Physics_Joint;

    public enum DegreeOfFreedom
    {
        Fixed_0D = 0,
        Hinge_1D = 1,
        BallAndSocket_3D = 3
    }

    public int Index { get; set; }
    public float Volume { get; set; }
    public float Stiffness { get; set; }
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }
    public DegreeOfFreedom DegreesOfFreedom { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Index));
            data.AddRange(BitConverter.GetBytes(Volume));
            data.AddRange(BitConverter.GetBytes(Stiffness));
            data.AddRange(BitConverter.GetBytes(MaxAngle));
            data.AddRange(BitConverter.GetBytes(MinAngle));
            data.AddRange(BitConverter.GetBytes((int)DegreesOfFreedom));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(int) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(int);

    public PhysicsJointChunk(BinaryReader br) : base(ChunkID)
    {
        Index = br.ReadInt32();
        Volume = br.ReadSingle();
        Stiffness = br.ReadSingle();
        MaxAngle = br.ReadSingle();
        MinAngle = br.ReadSingle();
        DegreesOfFreedom = (DegreeOfFreedom)br.ReadInt32();
    }

    public PhysicsJointChunk(int index, float volume, float stiffness, float maxAngle, float minAngle, DegreeOfFreedom degreesOfFreedom) : base(ChunkID)
    {
        Index = index;
        Volume = volume;
        Stiffness = stiffness;
        MaxAngle = maxAngle;
        MinAngle = minAngle;
        DegreesOfFreedom = degreesOfFreedom;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(Volume);
        bw.Write(Stiffness);
        bw.Write(MaxAngle);
        bw.Write(MinAngle);
        bw.Write((int)DegreesOfFreedom);
    }

    protected override Chunk CloneSelf() => new PhysicsJointChunk(Index, Volume, Stiffness, MaxAngle, MinAngle, DegreesOfFreedom);
}