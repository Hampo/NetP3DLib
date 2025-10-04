using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class ChunkCollection : Collection<Chunk>
{
    private readonly Chunk _owner;

    public ChunkCollection(Chunk owner)
    {
        _owner = owner;
    }

    public ChunkCollection(Chunk owner, int capacity) : base(new List<Chunk>(capacity))
    {
        _owner = owner;
    }

    protected override void InsertItem(int index, Chunk item)
    {
        if (item.ParentChunk != null)
            throw new InvalidOperationException($"Chunk \"{item}\" already has parent chunk. You must first remove it from \"{item.ParentChunk}\".");

        base.InsertItem(index, item);
        item.ParentChunk = _owner;
    }

    protected override void RemoveItem(int index)
    {
        Chunk old = this[index];
        base.RemoveItem(index);
        old.ParentChunk = null;
    }

    protected override void SetItem(int index, Chunk item)
    {
        Chunk old = this[index];

        if (old != null)
            old.ParentChunk = null;

        if (item.ParentChunk != null)
            throw new InvalidOperationException($"Chunk \"{item}\" already has parent chunk. You must first remove it from \"{item.ParentChunk}\".");

        base.SetItem(index, item);
        item.ParentChunk = _owner;
    }

    protected override void ClearItems()
    {
        foreach (var child in this)
            child.ParentChunk = null;

        base.ClearItems();
    }

    public void AddRange(IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
            Add(item);
    }
}
