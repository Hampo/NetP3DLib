using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class SizeAwareList<T> : ObservableCollection<T>
{
    private readonly Chunk _chunk;
    private readonly Action<uint> _onChange;

    public SizeAwareList(Chunk chunk, Action<uint> onChange, int capacity = 0) : base(new List<T>(capacity))
    {
        _chunk = chunk;
        _onChange = onChange ?? throw new ArgumentNullException(nameof(onChange));
    }

    private void TrackSizeChange(Action action)
    {
        uint oldSize = _chunk.HeaderSize;
        action();
        _onChange(oldSize);
    }

    protected override void InsertItem(int index, T item) => TrackSizeChange(() => base.InsertItem(index, item));

    protected override void RemoveItem(int index) => TrackSizeChange(() => base.RemoveItem(index));

    protected override void SetItem(int index, T item) => TrackSizeChange(() => base.SetItem(index, item));

    protected override void ClearItems() => TrackSizeChange(base.ClearItems);

    /// <summary>
    /// Adds a collection of items at the end.
    /// </summary>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        TrackSizeChange(() =>
        {
            foreach (var item in items)
                Items.Add(item);
        });
    }

    /// <summary>
    /// Inserts a collection of items starting at a specific index.
    /// </summary>
    public void InsertRange(int index, IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        TrackSizeChange(() =>
        {
            if (Items is List<T> list)
            {
                list.InsertRange(index, items);
            }
            else
            {
                int i = index;
                foreach (var item in items)
                    base.InsertItem(i++, item);
            }
        });
    }

    /// <summary>
    /// Removes a range of items starting at a specific index.
    /// </summary>
    public void RemoveRange(int index, int count)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (index + count > Count) throw new ArgumentException("Range exceeds list bounds.");

        TrackSizeChange(() =>
        {
            if (Items is List<T> list)
            {
                list.RemoveRange(index, count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                    base.RemoveItem(index);
            }
        });
    }

    /// <summary>Finds the index of the first element matching the predicate.</summary>
    public int FindIndex(Predicate<T> match)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));

        if (Items is List<T> list)
            return list.FindIndex(match);

        for (int i = 0; i < Count; i++)
            if (match(Items[i]))
                return i;

        return -1;
    }
}