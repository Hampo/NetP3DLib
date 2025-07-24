using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ScenegraphBranchChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Scenegraph_Branch;
    
    public uint Version { get; set; }
    public uint NumChildren => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumChildren));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public ScenegraphBranchChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        var numChildren = br.ReadUInt32();
    }

    public ScenegraphBranchChunk(uint version, string name) : base(ChunkID)
    {
        Version = version;
        Name = name;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(NumChildren);
    }

    internal override Chunk CloneSelf() => new ScenegraphBranchChunk(Version, Name);
}