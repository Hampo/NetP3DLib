using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawablePropListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Prop_List;
    
    public uint NumElements => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Composite_Drawable_Prop).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumElements));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CompositeDrawablePropListChunk(BinaryReader br) : base(ChunkID)
    {
        var numElements = br.ReadUInt32();
    }

    public CompositeDrawablePropListChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumElements);
    }

    internal override Chunk CloneSelf() => new CompositeDrawablePropListChunk();
}