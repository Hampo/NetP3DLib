using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ScenegraphRootChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Scenegraph_Root;

    public uint Version { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            return BitConverter.GetBytes(Version);
        }
    }
    public override uint DataLength => sizeof(uint);

    public ScenegraphRootChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
    }

    public ScenegraphRootChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
    }

    protected override Chunk CloneSelf() => new OldScenegraphRootChunk();
}