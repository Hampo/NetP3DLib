using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Fenceline)]
public class FencelineChunk : Chunk
{
    public uint NumWalls => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Wall).Count();

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
    public FencelineChunk(BinaryReader br) : base((uint)ChunkIdentifier.Fenceline)
    {
        var numWalls = br.ReadUInt32();
    }

    public FencelineChunk() : base((uint)ChunkIdentifier.Fenceline)
    { }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumWalls);
    }
}