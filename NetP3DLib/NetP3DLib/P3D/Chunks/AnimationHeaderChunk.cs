using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Animation_Header)]
public class AnimationHeaderChunk : Chunk
{
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

    public AnimationHeaderChunk(BinaryReader br) : base((uint)ChunkIdentifier.Animation_Header)
    {
        Version = br.ReadUInt32();
        NumGroups = br.ReadUInt32();
    }

    public AnimationHeaderChunk(uint version, uint numGroups) : base((uint)ChunkIdentifier.Animation_Header)
    {
        Version = version;
        NumGroups = numGroups;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumGroups);
    }
}