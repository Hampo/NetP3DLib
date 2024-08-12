using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Tree)]
public class TreeChunk : Chunk
{
    public uint NumChildren => (uint)Children.Count;
    public Vector3 Minimum { get; set; }
    public Vector3 Maximum { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumChildren));
            data.AddRange(BinaryExtensions.GetBytes(Minimum));
            data.AddRange(BinaryExtensions.GetBytes(Maximum));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public TreeChunk(BinaryReader br) : base((uint)ChunkIdentifier.Tree)
    {
        var numChildren = br.ReadUInt32();
        Minimum = br.ReadVector3();
        Maximum = br.ReadVector3();
    }

    public TreeChunk(Vector3 minimum, Vector3 maximum) : base((uint)ChunkIdentifier.Tree)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumChildren);
        bw.Write(Minimum);
        bw.Write(Maximum);
    }
}