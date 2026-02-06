using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TreeNodeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tree_Node;

    internal uint _cachedSubTreeSize;
    public uint SubTreeSize
    {
        get
        {
            if (ParentChunk is not TreeChunk treeChunk)
                return 0;

            treeChunk.RecalculateSubTreeSizeIfNeeded();
            return _cachedSubTreeSize;
        }
    }
    public int ParentOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(SubTreeSize));
            data.AddRange(BitConverter.GetBytes(ParentOffset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(int);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public TreeNodeChunk(BinaryReader br) : base(ChunkID)
    {
        var numChildren = br.ReadUInt32();
        ParentOffset = br.ReadInt32();
    }

    public TreeNodeChunk(int parentOffset) : base(ChunkID)
    {
        ParentOffset = parentOffset;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (Children.Count == 0)
            yield return new InvalidP3DException(this, $"There must be at least one Spatial Node child chunk.");
        foreach (var child in Children)
            if (child.ID != (uint)ChunkIdentifier.Spatial_Node)
                yield return new InvalidP3DException(this, $"Child chunk {child} is invalid. Child chunks must be an instance of Spatial Node.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(SubTreeSize);
        bw.Write(ParentOffset);
    }

    protected override Chunk CloneSelf() => new TreeNodeChunk( ParentOffset);
}