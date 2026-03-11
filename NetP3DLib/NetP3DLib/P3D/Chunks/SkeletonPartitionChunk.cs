using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;

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

    public SkeletonPartitionChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), ListHelper.ReadArray(br.ReadInt32(), br.ReadUInt32))
    {
    }

    public SkeletonPartitionChunk(string name, IList<uint> jointBits) : base(ChunkID, name)
    {
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