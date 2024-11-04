using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Skeleton_Partition)]
public class SkeletonPartitionChunk : NamedChunk
{
    public uint NumJointValues
    {
        get => (uint)JointBits.Count;
        set
        {
            if (value == NumJointValues)
                return;

            if (value < NumJointValues)
            {
                while (NumJointValues > value)
                    JointBits.RemoveAt(JointBits.Count - 1);
            }
            else
            {
                while (NumJointValues < value)
                    JointBits.Add(0);
            }
        }
    }
    public List<uint> JointBits { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumJointValues));
            foreach (var jointBit in JointBits)
                data.AddRange(BitConverter.GetBytes(jointBit));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) * NumJointValues;

    public SkeletonPartitionChunk(BinaryReader br) : base((uint)ChunkIdentifier.Skeleton_Partition)
    {
        Name = br.ReadP3DString();
        var numJointValues = br.ReadInt32();
        JointBits.Capacity = numJointValues;
        for (var i = 0; i < numJointValues; i++)
            JointBits.Add(br.ReadUInt32());
    }

    public SkeletonPartitionChunk(string name, IList<uint> jointBits) : base((uint)ChunkIdentifier.Skeleton_Partition)
    {
        Name = name;
        JointBits.AddRange(jointBits);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumJointValues);
        foreach (var jointBit in JointBits)
            bw.Write(jointBit);
    }
}