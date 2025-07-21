using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonPartitionChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Partition;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) * NumJointValues;

    public SkeletonPartitionChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numJointValues = br.ReadInt32();
        JointBits.Capacity = numJointValues;
        for (var i = 0; i < numJointValues; i++)
            JointBits.Add(br.ReadUInt32());
    }

    public SkeletonPartitionChunk(string name, IList<uint> jointBits) : base(ChunkID)
    {
        Name = name;
        JointBits.AddRange(jointBits);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumJointValues);
        foreach (var jointBit in JointBits)
            bw.Write(jointBit);
    }
}