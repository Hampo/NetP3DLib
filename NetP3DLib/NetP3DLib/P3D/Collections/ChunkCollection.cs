using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class ChunkCollection : Collection<Chunk>
{
    private readonly Chunk _owner;
    private uint _totalSize;

    public Chunk Owner => _owner;
    public uint TotalSize
    {
        get => _totalSize;
        internal set
        {
            if (value == _totalSize) return;

            uint old = _totalSize;
            _totalSize = value;

            if (value > old)
            {
                uint diff = value - old;
                PropagateSizeUp((int)diff);
            }
            else
            {
                uint diff = old - value;
                PropagateSizeUp(-checked((int)diff));
            }
        }
    }

    private void ValidateHierarchyOverflow(uint sizeToAdd)
    {
        if (sizeToAdd == 0)
            return;

        if (uint.MaxValue - _owner.Size < sizeToAdd)
            throw new OverflowException($"Adding size {sizeToAdd} would overflow chunk {_owner}.");

        var current = _owner;
        while (current != null)
        {
            if (current.ParentFile != null)
            {
                if (uint.MaxValue - current.ParentFile.Size < sizeToAdd)
                    throw new OverflowException($"Adding size {sizeToAdd} would overflow parent file {current.ParentFile}.");
                break;
            }

            if (current.ParentChunk != null)
            {
                if (uint.MaxValue - current.ParentChunk.Size < sizeToAdd)
                    throw new OverflowException($"Adding size {sizeToAdd} would overflow parent chunk {current.ParentChunk}.");

                current = current.ParentChunk;
            }
            else
            {
                break;
            }
        }
    }

    public ChunkCollection(Chunk owner, int capacity = 0) : base(new List<Chunk>(capacity))
    {
        _owner = owner;
    }

    protected override void InsertItem(int index, Chunk item)
    {
        if (item.ParentFile != null)
            throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentFile}\".");

        if (item.ParentChunk != null)
            throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentChunk}\".");

        ValidateHierarchyOverflow(item.Size);

        base.InsertItem(index, item);

        TotalSize += item.Size;
        item.ParentChunk = _owner;
        item.IndexInParent = index;

        UpdateChildIndices(index + 1);
    }

    protected override void RemoveItem(int index)
    {
        Chunk old = this[index];
        base.RemoveItem(index);

        TotalSize -= old.Size;
        old.ParentChunk = null;
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
            if (item.Size > old.Size)
            {
                uint diff = item.Size - old.Size;
                ValidateHierarchyOverflow(diff);
            }

            TotalSize -= old.Size;
            old.ParentChunk = null;
            old.IndexInParent = -1;
        }

        base.SetItem(index, item);
        TotalSize += item.Size;
        item.ParentChunk = _owner;
        item.IndexInParent = index;
    }

    protected override void ClearItems()
    {
        foreach (var child in this)
        {
            child.ParentChunk = null;
            child.IndexInParent = -1;
        }

        base.ClearItems();
        TotalSize = 0;
    }

    public IReadOnlyList<Chunk> GetRange(int index, int count) => ((List<Chunk>)Items).GetRange(index, count);

    public void AddRange(IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot be null.");

        var chunkList = items as ICollection<Chunk> ?? [.. items];

        uint addedSize = 0;
        foreach (var item in chunkList)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot contain any null entries.");

            if (item.ParentFile != null)
                throw new InvalidOperationException($"Cannot add chunk \"{item}\" into \"{_owner}\". It already belongs to \"{item.ParentFile}\".");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Cannot add chunk \"{item}\" into \"{_owner}\". It already belongs to \"{item.ParentChunk}\".");

            addedSize += item.Size;
        }
        ValidateHierarchyOverflow(addedSize);

        int startIndex = Count;

        ((List<Chunk>)Items).AddRange(items);
        TotalSize += addedSize;

        int i = startIndex;
        foreach (var item in chunkList)
        {
            item.ParentChunk = _owner;
            item.IndexInParent = i++;
        }

    }

    public void InsertRange(int index, IEnumerable<Chunk> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot be null.");

        var chunkList = items as ICollection<Chunk> ?? [.. items];

        uint addedSize = 0;
        foreach (var item in chunkList)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} cannot contain any null entries.");

            if (item.ParentFile != null)
                throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentFile}\".");

            if (item.ParentChunk != null)
                throw new InvalidOperationException($"Cannot insert chunk \"{item}\" into \"{_owner}\" at index {index}. It already belongs to \"{item.ParentChunk}\".");

            addedSize += item.Size;
        }
        ValidateHierarchyOverflow(addedSize);

        ((List<Chunk>)Items).InsertRange(index, chunkList);
        TotalSize += addedSize;

        foreach (var item in chunkList)
            item.ParentChunk = _owner;

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

        uint removedSize = 0;
        for (int i = index; i < index + count; i++)
        {
            var chunk = list[i];
            chunk.ParentChunk = null;
            chunk.IndexInParent = -1;
            removedSize += chunk.Size;
        }

        list.RemoveRange(index, count);
        TotalSize -= removedSize;
        if (index < Count)
            UpdateChildIndices(index);
    }

    private void UpdateChildIndices(int startIndex = 0)
    {
        for (int i = startIndex; i < Count; i++)
            this[i].IndexInParent = i;
    }

    private void PropagateSizeUp(int diff)
    {
        if (diff == 0) return;

        if (_owner.ParentChunk != null)
        {
            checked
            {
                if (diff > 0)
                    _owner.ParentChunk.Children.TotalSize += (uint)diff;
                else if (diff < 0)
                    _owner.ParentChunk.Children.TotalSize -= (uint)-(long)diff;
            }
        }
        else if (_owner.ParentFile != null)
        {
            checked
            {
                if (diff > 0)
                    _owner.ParentFile.Chunks.TotalSize += (uint)diff;
                else if (diff < 0)
                    _owner.ParentFile.Chunks.TotalSize -= (uint)-(long)diff;
            }
        }
    }
}
