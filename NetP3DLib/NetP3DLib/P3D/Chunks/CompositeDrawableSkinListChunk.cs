using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableSkinListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Skin_List;
    
    public uint NumElements => GetChildCount(ChunkIdentifier.Composite_Drawable_Skin);

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
    public CompositeDrawableSkinListChunk(BinaryReader br) : base(ChunkID)
    {
        var numElements = br.ReadUInt32();
    }

    public CompositeDrawableSkinListChunk() : base(ChunkID)
    { }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumElements);
    }

    internal override Chunk CloneSelf() => new CompositeDrawableSkinListChunk();
}