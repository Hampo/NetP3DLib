using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class ChunkCollection : Collection<Chunk>
{
    private readonly Chunk _owner;
    public Chunk Owner => _owner;

    public ChunkCollection(Chunk owner, int capacity = 0) : base(new List<Chunk>(capacity))
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
        foreach (var child in new List<Chunk>(this))
            child.ParentChunk = null;

        base.ClearItems();
    }

    public void AddRange(IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot be null.");

        var chunkList = items as ICollection<Chunk> ?? [.. items];

        foreach (var item in chunkList)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot contain any null entries.");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Chunk \"{item}\" already has parent chunk. You must first remove it from \"{item.ParentChunk}\".");
        }

        foreach (var item in chunkList)
            item.ParentChunk = _owner;

        ((List<Chunk>)Items).AddRange(items);
    }

    public void InsertRange(int index, IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot be null.");

        var chunkList = items as ICollection<Chunk> ?? [.. items];

        foreach (var item in chunkList)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot contain any null entries.");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Chunk \"{item}\" already has parent chunk. You must first remove it from \"{item.ParentChunk}\".");
        }

        foreach (var item in chunkList)
            item.ParentChunk = _owner;

        ((List<Chunk>)Items).InsertRange(index, chunkList);
    }
}
