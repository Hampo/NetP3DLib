using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Physics_Joint)]
public class PhysicsJointChunk : Chunk
{
    public uint Index { get; set; }
    public float Volume { get; set; }
    public float Stiffness { get; set; }
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }
    public float DOF { get; set; }

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
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float);

    public PhysicsJointChunk(BinaryReader br) : base((uint)ChunkIdentifier.Physics_Joint)
    {
        Index = br.ReadUInt32();
        Volume = br.ReadSingle();
        Stiffness = br.ReadSingle();
        MaxAngle = br.ReadSingle();
        MinAngle = br.ReadSingle();
        DOF = br.ReadSingle();
    }

    public PhysicsJointChunk(uint index, float volume, float stiffness, float maxAngle, float minAngle, float dof) : base((uint)ChunkIdentifier.Physics_Joint)
    {
        Index = index;
        Volume = volume;
        Stiffness = stiffness;
        MaxAngle = maxAngle;
        MinAngle = minAngle;
        DOF = dof;
    }

    public override void Validate()
    {
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
}