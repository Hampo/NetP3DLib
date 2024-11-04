using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TreeNodeChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Tree_Node;
    
    public uint NumChildren { get; set; } // TODO: This is calculated from the `NumChildren` and `ParentOffset` of other `TreeNodeChunk`s in the parent `Tree`. Currently no way to access a parent from the current chunk so cannot be calculated at this time.
    public int ParentOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumChildren));
            data.AddRange(BitConverter.GetBytes(ParentOffset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(int);

    public TreeNodeChunk(BinaryReader br) : base(ChunkID)
    {
        NumChildren = br.ReadUInt32();
        ParentOffset = br.ReadInt32();
    }

    public TreeNodeChunk(uint numChildren, int parentOffset) : base(ChunkID)
    {
        NumChildren = numChildren;
        ParentOffset = parentOffset;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumChildren);
        bw.Write(ParentOffset);
    }
}