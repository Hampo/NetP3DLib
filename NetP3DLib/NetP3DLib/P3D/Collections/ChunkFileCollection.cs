using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class ChunkFileCollection : Collection<Chunk>
{
    private readonly P3DFile _owner;
    private uint _totalSize;

    public P3DFile Owner => _owner;
    public uint TotalSize
    {
        //get => _totalSize;
        get
        {
            var size = 0u;
            foreach (var chunk in this)
                size += chunk.Size;
            return size;
        }
        internal set
        {
            return;
            if (value > _totalSize)
            {
                uint diff = value - _totalSize;
                if (int.MaxValue - _owner.Size < diff)
                    throw new OverflowException($"Size update would overflow File size.");
            }
            _totalSize = value;
        }
    }

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

        if (int.MaxValue - _owner.Size < item.Size)
            throw new OverflowException($"Adding chunk size {item.Size} would overflow owner size {_owner.Size}.");

        base.InsertItem(index, item);

        _totalSize += item.Size;
        item.ParentFile = _owner;
        item.IndexInParent = index;

        item.SizeChanged += OnChildSizeChanged;

        UpdateChildIndices(index + 1);
    }

    protected override void RemoveItem(int index)
    {
        Chunk old = this[index];
        old.SizeChanged -= OnChildSizeChanged;

        base.RemoveItem(index);

        _totalSize -= old.Size;
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
            if (item.Size > old.Size)
            {
                uint diff = item.Size - old.Size;
                if (int.MaxValue - _owner.Size < diff)
                    throw new OverflowException($"Replacing chunk at index {index} would overflow owner size.");
            }

            _totalSize -= old.Size;
            old.ParentFile = null;
            old.IndexInParent = -1;
            old.SizeChanged -= OnChildSizeChanged;
        }

        base.SetItem(index, item);
        _totalSize += item.Size;
        item.ParentFile = _owner;
        item.IndexInParent = index;
        item.SizeChanged += OnChildSizeChanged;
    }

    protected override void ClearItems()
    {
        foreach (var child in this)
        {
            child.ParentFile = null;
            child.IndexInParent = -1;
            child.SizeChanged -= OnChildSizeChanged;
        }

        base.ClearItems();
        _totalSize = 0;
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

        if (int.MaxValue - _owner.Size < addedSize)
            throw new OverflowException($"Adding chunks with total size {addedSize} would overflow owner size {_owner.Size}.");

        int startIndex = Count;
        ((List<Chunk>)Items).AddRange(items);
        _totalSize += addedSize;

        int i = startIndex;
        foreach (var item in chunkList)
        {
            item.ParentFile = _owner;
            item.IndexInParent = i++;
            item.SizeChanged += OnChildSizeChanged;
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

        if (int.MaxValue - _owner.Size < addedSize)
            throw new OverflowException($"Adding chunks with total size {addedSize} would overflow owner size {_owner.Size}.");

        ((List<Chunk>)Items).InsertRange(index, chunkList);
        _totalSize += addedSize;

        foreach (var item in chunkList)
        {
            item.ParentFile = _owner;
            item.SizeChanged += OnChildSizeChanged;
        }

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
            chunk.ParentFile = null;
            chunk.IndexInParent = -1;
            removedSize += chunk.Size;
            chunk.SizeChanged -= OnChildSizeChanged;
        }

        list.RemoveRange(index, count);
        _totalSize -= removedSize;
        if (index < Count)
            UpdateChildIndices(index);
    }

    private void UpdateChildIndices(int startIndex = 0)
    {
        for (int i = startIndex; i < Count; i++)
            this[i].IndexInParent = i;
    }

    private void OnChildSizeChanged(Chunk child, int delta)
    {
        if (delta > 0)
            TotalSize += (uint)delta;
        else if (delta < 0)
            TotalSize -= (uint)-(long)delta;
    }
}
