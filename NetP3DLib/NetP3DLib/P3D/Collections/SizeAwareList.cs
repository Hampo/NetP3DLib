using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetP3DLib.P3D.Collections;
public class SizeAwareList<T> : ObservableCollection<T>
{
    private readonly Action _onChange;
    private bool _suspendNotifications = false;

    public SizeAwareList(Action onChange, int capacity = 0) : base(new List<T>(capacity))
    {
        _onChange = onChange ?? throw new ArgumentNullException(nameof(onChange));
    }

    public void SuspendNotifications() => _suspendNotifications = true;

    public void ResumeNotifications()
    {
        _suspendNotifications = false;
        _onChange();
        OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
    }

    protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (_suspendNotifications)
            return;
        base.OnCollectionChanged(e);
    }

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        if (!_suspendNotifications)
            _onChange();
    }

    protected override void RemoveItem(int index)
    {
        base.RemoveItem(index);
        if (!_suspendNotifications)
            _onChange();
    }

    protected override void SetItem(int index, T item)
    {
        base.SetItem(index, item);
        if (!_suspendNotifications)
            _onChange();
    }

    protected override void ClearItems()
    {
        base.ClearItems();
        if (!_suspendNotifications)
            _onChange();
    }

    /// <summary>
    /// Adds a collection of items at the end.
    /// </summary>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
            Items.Add(item);

        if (!_suspendNotifications)
            _onChange();
    }

    /// <summary>
    /// Inserts a collection of items starting at a specific index.
    /// </summary>
    public void InsertRange(int index, IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        if (Items is List<T> list)
        {
            list.InsertRange(index, items);
            if (!_suspendNotifications)
                _onChange();
        }
        else
        {
            int i = index;
            foreach (var item in items)
                base.InsertItem(i++, item);
        }
    }

    /// <summary>
    /// Removes a range of items starting at a specific index.
    /// </summary>
    public void RemoveRange(int index, int count)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (index + count > Count) throw new ArgumentException("Range exceeds list bounds.");

        if (Items is List<T> list)
        {
            list.RemoveRange(index, count);
            if (!_suspendNotifications)
                _onChange();
        }
        else
        {
            for (int i = 0; i < count; i++)
                base.RemoveItem(index);
        }
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