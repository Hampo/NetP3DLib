using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableSortOrderChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Sort_Order;
    
    public float SortOrder { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(SortOrder));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float);

    public CompositeDrawableSortOrderChunk(BinaryReader br) : base(ChunkID)
    {
        SortOrder = br.ReadSingle();
    }

    public CompositeDrawableSortOrderChunk(float sortOrder) : base(ChunkID)
    {
        SortOrder = sortOrder;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(SortOrder);
    }
}