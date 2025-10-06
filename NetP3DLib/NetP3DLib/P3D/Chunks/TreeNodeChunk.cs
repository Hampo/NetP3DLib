using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TreeNodeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tree_Node;

    internal uint _cachedNumChildren;
    public uint NumChildren
    {
        get
        {
            if (ParentChunk is not TreeChunk treeChunk)
                return 0;

            treeChunk.RecalculateNumChildrenIfNeeded();
            return _cachedNumChildren;
        }
    }
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

    public override void Validate()
    {
        if (Children.Count == 0)
            throw new InvalidDataException($"There must be at least one Tree Node 2 child chunk.");
        foreach (var child in Children)
            if (child.ID != (uint)ChunkIdentifier.Tree_Node_2)
                throw new InvalidDataException($"Child chunk {child} is invalid. Child chunks must be an instance of Tree Node 2.");

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumChildren);
        bw.Write(ParentOffset);
    }

    protected override Chunk CloneSelf() => new TreeNodeChunk( ParentOffset);
}