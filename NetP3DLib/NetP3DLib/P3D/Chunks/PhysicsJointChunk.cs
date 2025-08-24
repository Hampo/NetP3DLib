using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhysicsJointChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Physics_Joint;
    
    public uint Index { get; set; }
    public float Volume { get; set; }
    public float Stiffness { get; set; }
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }
    public int DOF { get; set; }

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
            data.AddRange(BitConverter.GetBytes(DOF));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(int);

    public PhysicsJointChunk(BinaryReader br) : base(ChunkID)
    {
        Index = br.ReadUInt32();
        Volume = br.ReadSingle();
        Stiffness = br.ReadSingle();
        MaxAngle = br.ReadSingle();
        MinAngle = br.ReadSingle();
        DOF = br.ReadInt32();
    }

    public PhysicsJointChunk(uint index, float volume, float stiffness, float maxAngle, float minAngle, int dof) : base(ChunkID)
    {
        Index = index;
        Volume = volume;
        Stiffness = stiffness;
        MaxAngle = maxAngle;
        MinAngle = minAngle;
        DOF = dof;
    }

    private static readonly HashSet<int> ValidDOF = [0, 1, 3];
    public override void Validate()
    {
        if (!ValidDOF.Contains(DOF))
            throw new InvalidDataException($"{nameof(DOF)} must be one of: {string.Join(", ", ValidDOF)}");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(Volume);
        bw.Write(Stiffness);
        bw.Write(MaxAngle);
        bw.Write(MinAngle);
        bw.Write(DOF);
    }

    internal override Chunk CloneSelf() => new PhysicsJointChunk(Index, Volume, Stiffness, MaxAngle, MinAngle, DOF);
}