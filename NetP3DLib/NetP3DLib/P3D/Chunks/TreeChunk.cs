using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TreeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tree;
    
    public uint NumChildren => GetChildCount(ChunkIdentifier.Tree_Node);
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
    public TreeChunk(BinaryReader br) : base(ChunkID)
    {
        var numChildren = br.ReadUInt32();
        Minimum = br.ReadVector3();
        Maximum = br.ReadVector3();
    }

    public TreeChunk(Vector3 minimum, Vector3 maximum) : base(ChunkID)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumChildren);
        bw.Write(Minimum);
        bw.Write(Maximum);
    }

    protected override Chunk CloneSelf() => new TreeChunk(Minimum, Maximum);

    private int _childHash = 0;
    private int _childCount = 0;
    internal void RecalculateNumChildrenIfNeeded()
    {
        var children = GetChunksOfType<TreeNodeChunk>();
        var childCount = children.Count;

        unchecked
        {
            int hash = 17;
            for (int i = 0; i < childCount; i++)
                hash = hash * 31 + children[i].ParentOffset;

            if (hash == _childHash && childCount == _childCount)
                return;

            _childHash = hash;
            _childCount = childCount;
        }

        ComputeNumChildrenForAllNodes(children);
    }

    private void ComputeNumChildrenForAllNodes(IReadOnlyList<TreeNodeChunk> children)
    {
        foreach (var c in children)
            c?._cachedNumChildren = 0;

        for (int i = children.Count - 1; i >= 0; i--)
        {
            var node = children[i];
            if (node == null)
                continue;

            if (node.ParentOffset == 0)
                continue;

            int parentIndex = i + node.ParentOffset;
            if (parentIndex >= 0 && parentIndex < children.Count && children[parentIndex] is TreeNodeChunk parent)
                parent._cachedNumChildren += 1 + node._cachedNumChildren;
        }
    }
}