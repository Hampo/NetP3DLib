using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
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
        get => (uint)(JointBits?.Count ?? 0);
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
    public SizeAwareList<uint> JointBits { get; }

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
        _name = new(this, br);
        var numJointValues = br.ReadInt32();
        var jointBits = new uint[numJointValues];
        for (var i = 0; i < numJointValues; i++)
            jointBits[i] = br.ReadUInt32();
        JointBits = CreateSizeAwareList(jointBits);
    }

    public SkeletonPartitionChunk(string name, IList<uint> jointBits) : base(ChunkID)
    {
        _name = new(this, name);
        JointBits = CreateSizeAwareList(jointBits);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumJointValues);
        foreach (var jointBit in JointBits)
            bw.Write(jointBit);
    }

    protected override Chunk CloneSelf() => new SkeletonPartitionChunk(Name, JointBits);
}