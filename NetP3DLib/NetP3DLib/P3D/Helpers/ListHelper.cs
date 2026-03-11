using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Helpers;
public static class ListHelper
{
    private static int FindInsertIndex<T>(List<T> list, int indexInParent) where T : Chunk
    {
        int low = 0;
        int high = list.Count;

        while (low < high)
        {
            int mid = low + (high - low) / 2;
            if (list[mid].IndexInParent < indexInParent)
                low = mid + 1;
            else
                high = mid;
        }
        return low;
    }

    public static void InsertSorted<T>(List<T> list, T chunk) where T : Chunk
    {
        int index = FindInsertIndex(list, chunk.IndexInParent);
        list.Insert(index, chunk);
    }

    public static void InsertSorted<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key, TValue chunk) where TValue : Chunk
    {
        if (!dict.TryGetValue(key, out var list))
        {
            list = [];
            dict[key] = list;
        }
        InsertSorted(list, chunk);
    }

    public static void CleanListInDictionary<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key) where TValue : Chunk
    {
        if (!dict.TryGetValue(key, out var list))
            return;

        for (var i = list.Count - 1; i >= 0; i--)
            if (list[i].IndexInParent == -1)
                list.RemoveAt(i);

        if (list.Count == 0)
            dict.Remove(key);
    }

    public static T[] ReadArray<T>(Func<int> getCount, Func<T> readValue, out int count)
    {
        count = getCount();
        var results = new T[count];
        for (var i = 0; i < count; i++)
            results[i] = readValue();
        return results;
    }

    public static T[] ReadArray<T>(int count, Func<T> readValue)
    {
        var results = new T[count];
        for (var i = 0; i < count; i++)
            results[i] = readValue();
        return results;
    }
}
