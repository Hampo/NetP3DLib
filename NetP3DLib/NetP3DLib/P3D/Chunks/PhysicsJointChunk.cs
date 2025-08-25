using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhysicsJointChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Physics_Joint;

    public enum DegreesOfFreedom
    {
        Fixed0D = 0,
        Hinge1D = 1,
        BallAndSocket3D = 3
    }

    public int Index { get; set; }
    public float Volume { get; set; }
    public float Stiffness { get; set; }
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }
    public DegreesOfFreedom DOF { get; set; }

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
            data.AddRange(BitConverter.GetBytes((int)DOF));

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
        DOF = (DegreesOfFreedom)br.ReadInt32();
    }

    public PhysicsJointChunk(int index, float volume, float stiffness, float maxAngle, float minAngle, DegreesOfFreedom dof) : base(ChunkID)
    {
        Index = index;
        Volume = volume;
        Stiffness = stiffness;
        MaxAngle = maxAngle;
        MinAngle = minAngle;
        DOF = dof;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(Volume);
        bw.Write(Stiffness);
        bw.Write(MaxAngle);
        bw.Write(MinAngle);
        bw.Write((int)DOF);
    }

    internal override Chunk CloneSelf() => new PhysicsJointChunk(Index, Volume, Stiffness, MaxAngle, MinAngle, DOF);
}