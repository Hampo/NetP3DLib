using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Skeleton)]
public class SkeletonChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint NumJoints => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Skeleton_Joint).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumJoints));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SkeletonChunk(BinaryReader br) : base((uint)ChunkIdentifier.Skeleton)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numJoints = br.ReadUInt32();
    }

    public SkeletonChunk(string name, uint version) : base((uint)ChunkIdentifier.Skeleton)
    {
        Name = name;
        Version = version;
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
    }
}