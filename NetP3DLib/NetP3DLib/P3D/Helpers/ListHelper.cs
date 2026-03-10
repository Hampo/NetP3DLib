using System.Collections.Generic;

namespace NetP3DLib.P3D.Helpers;
public static class ListHelper
{
    public static void InsertSorted(List<Chunk> list, Chunk chunk)
    {
        int index = 0;
        while (index < list.Count && list[index].IndexInParent < chunk.IndexInParent)
            index++;

        list.Insert(index, chunk);
    }

    public static void InsertSorted(List<NamedChunk> list, NamedChunk chunk)
    {
        int index = 0;
        while (index < list.Count && list[index].IndexInParent < chunk.IndexInParent)
            index++;

        list.Insert(index, chunk);
    }

    public static void InsertSorted(List<ParamChunk> list, ParamChunk chunk)
    {
        int index = 0;
        while (index < list.Count && list[index].IndexInParent < chunk.IndexInParent)
            index++;

        list.Insert(index, chunk);
    }
}
