using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TreeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tree;

    public uint NumNodes => GetChildCount(ChunkIdentifier.Tree_Node);
    public Vector3 BoundsMin { get; set; }
    public Vector3 BoundsMax { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumNodes));
            data.AddRange(BinaryExtensions.GetBytes(BoundsMin));
            data.AddRange(BinaryExtensions.GetBytes(BoundsMax));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public TreeChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        var numChildren = br.ReadUInt32();
        BoundsMin = br.ReadVector3();
        BoundsMax = br.ReadVector3();

        ChildAdded += Chunk_ChildAdded;
        ChildRemoved += Chunk_ChildRemoved;
        ChildrenAdded += Chunk_ChildrenAdded;
        ChildrenRemoved += Chunk_ChildrenRemoved;
        ChildrenCleared += Chunk_ChildrenCleared;
    }

    public TreeChunk(Vector3 minimum, Vector3 maximum) : base(ChunkID)
    {
        BoundsMin = minimum;
        BoundsMax = maximum;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumNodes);
        bw.Write(BoundsMin);
        bw.Write(BoundsMax);
    }

    protected override Chunk CloneSelf() => new TreeChunk(BoundsMin, BoundsMax);

    private bool _needsNewNodeList = true;
    private bool _needsRecalculate = true;
    private IReadOnlyList<TreeNodeChunk>? _cachedNodeList = null;

    internal void MarkDirty() => _needsRecalculate = true;

    internal void MarkTopologyDirty()
    {
        _needsNewNodeList = true;
        _needsRecalculate = true;
    }

    private void Chunk_ChildAdded(Chunk child)
    {
        if (child is TreeNodeChunk)
            MarkTopologyDirty();
    }

    private void Chunk_ChildRemoved(Chunk child, int oldIndex)
    {
        if (child is TreeNodeChunk)
            MarkTopologyDirty();
    }

    private void Chunk_ChildrenAdded(IReadOnlyList<Chunk> children)
    {
        foreach (var child in children)
        {
            if (child is TreeNodeChunk)
            {
                MarkTopologyDirty();
                return;
            }
        }
    }

    private void Chunk_ChildrenRemoved(IReadOnlyList<(Chunk child, int oldIndex)> children)
    {
        foreach (var (child, _) in children)
        {
            if (child is TreeNodeChunk)
            {
                MarkTopologyDirty();
                return;
            }
        }
    }

    private void Chunk_ChildrenCleared() => MarkTopologyDirty();

    internal void RecalculateSubTreeSizeIfNeeded()
    {
        if (_needsNewNodeList || _cachedNodeList == null)
        {
            _cachedNodeList = GetChunksOfType<TreeNodeChunk>();
            _needsNewNodeList = false;
        }

        if (_needsRecalculate)
        {
            ComputeSubTreeSizeForAllNodes();
            _needsRecalculate = false;
        }
    }

    private void ComputeSubTreeSizeForAllNodes()
    {
        var nodes = _cachedNodeList;
        if (nodes == null)
            return;

        for (int i = 0; i < nodes.Count; i++)
            nodes[i]._cachedSubTreeSize = 0;

        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var node = nodes[i];
            if (node.ParentOffset == 0)
                continue;

            int parentIndex = i + node.ParentOffset;
            if (parentIndex >= 0 && parentIndex < nodes.Count)
                nodes[parentIndex]._cachedSubTreeSize += 1 + node._cachedSubTreeSize;
        }
    }
}