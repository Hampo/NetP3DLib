using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class ChunkFileCollection : Collection<Chunk>
{
    private readonly P3DFile _owner;
    public P3DFile Owner => _owner;

    public ChunkFileCollection(P3DFile owner, int capacity = 0) : base(new List<Chunk>(capacity))
    {
        _owner = owner;
    }

    protected override void InsertItem(int index, Chunk item)
    {
        if (item.ParentFile != null)
            throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentFile}\".");

        if (item.ParentChunk != null)
            throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentChunk}\".");

        base.InsertItem(index, item);
        item.ParentFile = _owner;
        item.IndexInParent = index;

        UpdateChildIndices(index + 1);
    }

    protected override void RemoveItem(int index)
    {
        Chunk old = this[index];
        base.RemoveItem(index);
        old.ParentFile = null;
        old.IndexInParent = -1;

        UpdateChildIndices(index);
    }

    protected override void SetItem(int index, Chunk item)
    {
        if (item.ParentFile != null)
            throw new InvalidOperationException($"Cannot set chunk \"{item}\" in \"{_owner}\" at index {index}. It already belongs to \"{item.ParentFile}\".");

        if (item.ParentChunk != null)
            throw new InvalidOperationException($"Cannot set chunk \"{item}\" in \"{_owner}\" at index {index}. It already belongs to \"{item.ParentChunk}\".");

        Chunk old = this[index];

        if (old != null)
        {
            old.ParentFile = null;
            old.IndexInParent = -1;
        }

        base.SetItem(index, item);
        item.ParentFile = _owner;
        item.IndexInParent = index;
    }

    protected override void ClearItems()
    {
        foreach (var child in new List<Chunk>(this))
        {
            child.ParentFile = null;
            child.IndexInParent = -1;
        }

        base.ClearItems();
    }

    public IReadOnlyList<Chunk> GetRange(int index, int count) => ((List<Chunk>)Items).GetRange(index, count);

    public void AddRange(IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot be null.");

        var chunkList = items as ICollection<Chunk> ?? [.. items];

        foreach (var item in chunkList)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot contain any null entries.");

            if (item.ParentFile != null)
                throw new InvalidOperationException($"Cannot add chunk \"{item}\" into \"{_owner}\". It already belongs to \"{item.ParentFile}\".");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Cannot add chunk \"{item}\" into \"{_owner}\". It already belongs to \"{item.ParentChunk}\".");
        }

        int startIndex = Count;

        ((List<Chunk>)Items).AddRange(items);

        int i = startIndex;
        foreach (var item in chunkList)
        {
            item.ParentFile = _owner;
            item.IndexInParent = i++;
        }
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

            if (item.ParentFile != null)
                throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentFile}\".");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentChunk}\".");
        }

        ((List<Chunk>)Items).InsertRange(index, chunkList);

        foreach (var item in chunkList)
            item.ParentFile = _owner;

        UpdateChildIndices(index);
    }

    public void RemoveRange(int index, int count)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be negative.");

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} cannot be negative.");

        if (index + count > Count)
            throw new ArgumentException("The range defined by index and count exceeds the collection bounds.");

        if (count == 0)
            return;

        var list = (List<Chunk>)Items;

        for (int i = index; i < index + count; i++)
        {
            var chunk = list[i];
            chunk.ParentFile = null;
            chunk.IndexInParent = -1;
        }

        list.RemoveRange(index, count);
        if (index < Count)
            UpdateChildIndices(index);
    }

    private void UpdateChildIndices(int startIndex = 0)
    {
        for (int i = startIndex; i < Count; i++)
            this[i].IndexInParent = i;
    }
}
