using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphSortOrderChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Old_Scenegraph_Sort_Order;
    
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

    public OldScenegraphSortOrderChunk(BinaryReader br) : base(ChunkID)
    {
        SortOrder = br.ReadSingle();
    }

    public OldScenegraphSortOrderChunk(float sortOrder) : base(ChunkID)
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