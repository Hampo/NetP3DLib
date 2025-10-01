using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint NumJoints => GetChildCount(ChunkIdentifier.Skeleton_Joint);

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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SkeletonChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numJoints = br.ReadUInt32();
    }

    public SkeletonChunk(string name, uint version) : base(ChunkID)
    {
        Name = name;
        Version = version;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumJoints);
    }

    protected override Chunk CloneSelf() => new SkeletonChunk(Name, Version);
}
