using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationHeaderChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation_Header;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint NumGroups { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumGroups));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public AnimationHeaderChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        NumGroups = br.ReadUInt32();
    }

    public AnimationHeaderChunk(uint version, uint numGroups) : base(ChunkID)
    {
        Version = version;
        NumGroups = numGroups;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumGroups);
    }

    protected override Chunk CloneSelf() => new AnimationHeaderChunk(Version, NumGroups);
}
