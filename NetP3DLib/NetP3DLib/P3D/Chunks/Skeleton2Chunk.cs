using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Skeleton_2)]
public class Skeleton2Chunk : NamedChunk
{
    public uint Version { get; set; }
    public uint NumJoints => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Skeleton_Joint_2).Count();
    public uint NumPartitions => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Skeleton_Partition).Count();
    // TODO: Check if these are somehow automated
    public uint NumLimbs { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumJoints));
            data.AddRange(BitConverter.GetBytes(NumPartitions));
            data.AddRange(BitConverter.GetBytes(NumLimbs));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public Skeleton2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Skeleton_2)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numJoints = br.ReadUInt32();
        var numPartitions = br.ReadUInt32();
        NumLimbs = br.ReadUInt32();
    }

    public Skeleton2Chunk(string name, uint version, uint numLimbs) : base((uint)ChunkIdentifier.Skeleton_2)
    {
        Name = name;
        Version = version;
        NumLimbs = numLimbs;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumJoints);
        bw.Write(NumPartitions);
        bw.Write(NumLimbs);
    }
}