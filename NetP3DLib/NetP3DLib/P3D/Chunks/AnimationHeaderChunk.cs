using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationHeaderChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation_Header;
    
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

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumGroups);
    }
}