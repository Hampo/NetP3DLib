using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Composite_Drawable_Effect_List)]
public class CompositeDrawableEffectListChunk : Chunk
{
    public uint NumElements => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Composite_Drawable_Effect).Count();

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
    public CompositeDrawableEffectListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Composite_Drawable_Effect_List)
    {
        var numElements = br.ReadUInt32();
    }

    public CompositeDrawableEffectListChunk() : base((uint)ChunkIdentifier.Composite_Drawable_Effect_List)
    { }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumElements);
    }
}