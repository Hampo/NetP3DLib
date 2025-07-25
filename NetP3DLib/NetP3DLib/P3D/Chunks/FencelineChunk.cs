using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FencelineChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Fenceline;
    
    public uint NumWalls => GetChildCount(ChunkIdentifier.Wall);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumWalls));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public FencelineChunk(BinaryReader br) : base(ChunkID)
    {
        var numWalls = br.ReadUInt32();
    }

    public FencelineChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumWalls);
    }

    internal override Chunk CloneSelf() => new FencelineChunk();
}