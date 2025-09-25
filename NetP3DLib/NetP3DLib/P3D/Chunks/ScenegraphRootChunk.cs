using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.IO;

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

    public ScenegraphRootChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
    }

    public ScenegraphRootChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
    }

    internal override Chunk CloneSelf() => new OldScenegraphRootChunk();
}