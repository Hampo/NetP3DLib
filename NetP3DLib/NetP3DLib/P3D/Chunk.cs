using NetP3DLib.IO;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NetP3DLib.P3D;

/// <summary>
/// Class <c>Chunk</c> is the base chunk used for any unknown chunks.
/// </summary>
public abstract class Chunk
{
    /// <summary>
    /// Property <c>ID</c> is the chunk's identifier.
    /// </summary>
    public uint ID { get; set; }
    /// <summary>
    /// The parent file of this chunk. If <c>null</c>, chunk is not in a file.
    /// </summary>
    public P3DFile? ParentFile { get; internal set; } = null;
    /// <summary>
    /// The parent of this chunk. If <c>null</c>, either not in a hierarchy or in root of file (see <see cref="ParentFile"/>).
    /// </summary>
    public Chunk? ParentChunk { get; internal set; } = null;
    /// <summary>
    /// The index in <see cref="ParentChunk"/> or <see cref="ParentFile"/>. <c>-1</c> if no parent.
    /// </summary>
    public int IndexInParent { get; internal set; } = -1;
    /// <summary>
    /// Property <c>Children</c> is a collection of a chunk's children.
    /// </summary>
    public ChunkCollection Children { get; internal set; }
    /// <summary>
    /// Property <c>AllChildren</c> is all the chunk's children flattened.
    /// </summary>
    public ReadOnlyCollection<Chunk> AllChildren
    {
        get
        {
            var result = new List<Chunk>(Children.Count * 4);

            var stack = new Stack<Chunk>(Children.Count);
            for (int i = Children.Count - 1; i >= 0; i--)
                stack.Push(Children[i]);

            while (stack.Count > 0)
            {
                var chunk = stack.Pop();
                result.Add(chunk);

                var children = chunk.Children;
                for (int i = children.Count - 1; i >= 0; i--)
                    stack.Push(children[i]);
            }

            return result.AsReadOnly();
        }
    }

    /// <summary>
    /// Property <c>DataBytes</c> is the chunk's header data, built from the chunk's properties.
    /// <para>NOTE: This will use the system's default <see cref="Endianness"/>.</para>
    /// </summary>
    public abstract byte[] DataBytes { get; }
    /// <summary>
    /// Property <c>DataLength</c> should be the number of bytes of <see cref="DataBytes"/>.
    /// <para>NOTE: This can be calculated by <see cref="DataBytes"/>.<c>Length</c>, however hardcoding is faster.</para>
    /// </summary>
    public virtual uint DataLength => (uint)DataBytes.Length;
    /// <summary>
    /// Property <c>HeaderSize</c> is the chunk's header size.
    /// <para>This is usually <see cref="P3DFile.HEADER_SIZE"/> + <see cref="DataBytes"/><c>.Length</c>.</para>
    /// </summary>
    public uint HeaderSize => P3DFile.HEADER_SIZE + DataLength;
    /// <summary>
    /// Property <c>Size</c> is the chunk's size.
    /// <para>This is usually <see cref="HeaderSize"/> + SUM(<see cref="Children"/>.<see cref="Size"/>).</para>
    /// </summary>
    public uint Size => HeaderSize + Children.TotalSize;
    /// <summary>
    /// Property <c>Bytes</c> is the chunk's data, built from the chunk's properties and children.
    /// <para>NOTE: This will use the system's default <see cref="Endianness"/>.</para>
    /// </summary>
    public byte[] Bytes
    {
        get
        {
            List<byte> bytes = [];
            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(HeaderSize));
            bytes.AddRange(BitConverter.GetBytes(Size));
            bytes.AddRange(DataBytes);
            foreach (var chunk in Children)
                bytes.AddRange(chunk.Bytes);
            return [.. bytes];
        }
    }

    /// <summary>
    /// This constructor initialises an unknown chunk with the given <paramref name="chunkId"/>.
    /// </summary>
    /// <param name="chunkId">The chunk's identifier. See <seealso cref="ChunkIdentifier"/> for known identifiers.</param>
    protected Chunk(uint chunkId)
    {
        ID = chunkId;
        Children = new(this);
    }

    /// <summary>
    /// This constructor initialises an unknown chunk with the given <paramref name="chunkId"/>.
    /// </summary>
    /// <param name="chunkId">The chunk's identifier. See <seealso cref="ChunkIdentifier"/> for known identifiers.</param>
    protected Chunk(ChunkIdentifier chunkId) : this((uint)chunkId) { }

    /// <summary>
    /// Runs validation on this chunk and all <see cref="Children"/>. Throws an exception if any invalid data is found.
    /// </summary>
    /// <exception cref="System.IO.InvalidDataException">
    /// Thrown when any of the chunk's properties are invalid for a Pure3D file.
    /// </exception>
    public virtual void Validate()
    {
        foreach (var chunk in Children)
            chunk.Validate();
    }

    /// <summary>
    /// Writes this chunk, including its <see cref="Children"/>, to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    public void Write(System.IO.BinaryWriter bw)
    {
        bw.Write(ID);
        bw.Write(HeaderSize);
        bw.Write(Size);

#if DEBUG
        var startPos = bw.BaseStream.Position;
#endif
        WriteData(bw);
#if DEBUG
        var length = bw.BaseStream.Position - startPos;
        if (length != HeaderSize - 12)
        {
            ChunkIdentifier chunkIdentifier = (ChunkIdentifier)ID;
            Debugger.Break();
        }
#endif

        foreach (var chunk in Children)
            chunk.Write(bw);
    }

    public Chunk Clone()
    {
        var chunk = CloneSelf();
        foreach (var child in Children)
            chunk.Children.Add(child.Clone());
        return chunk;
    }

    public T? GetFirstChunkOfType<T>() where T : Chunk
    {
        foreach (var child in Children)
            if (child is T chunk)
                return chunk;
        return null;
    }

    public T? GetFirstChunkOfType<T>(string name) where T : NamedChunk
    {
        foreach (var child in Children)
            if (child is T chunk && chunk.Name == name)
                return chunk;
        return null;
    }

    public T? GetFirstParamOfType<T>(string param) where T : ParamChunk
    {
        foreach (var child in Children)
            if (child is T chunk && chunk.Param == param)
                return chunk;
        return null;
    }

    public T? GetLastChunkOfType<T>() where T : Chunk
    {
        for (int i = Children.Count - 1; i >= 0; i--)
            if (Children[i] is T chunk)
                return chunk;
        return null;
    }

    public T? GetLastChunkOfType<T>(string name) where T : NamedChunk
    {
        for (int i = Children.Count - 1; i >= 0; i--)
            if (Children[i] is T chunk && chunk.Name == name)
                return chunk;
        return null;
    }

    public T? GetLastParamOfType<T>(string param) where T : ParamChunk
    {
        for (int i = Children.Count - 1; i >= 0; i--)
            if (Children[i] is T chunk && chunk.Param == param)
                return chunk;
        return null;
    }

    public IReadOnlyList<T> GetChunksOfType<T>() where T : Chunk
    {
        var result = new List<T>();
        foreach (var child in Children)
        {
            if (child is T chunk)
                result.Add(chunk);
        }
        return result;
    }

    public IReadOnlyList<T> GetChunksOfType<T>(string name) where T : NamedChunk
    {
        var result = new List<T>();
        foreach (var child in Children)
        {
            if (child is T chunk && chunk.Name == name)
                result.Add(chunk);
        }
        return result;
    }

    public IReadOnlyList<T> GetParamsOfType<T>(string param) where T : ParamChunk
    {
        var result = new List<T>();
        foreach (var child in Children)
        {
            if (child is T chunk && chunk.Param == param)
                result.Add(chunk);
        }
        return result;
    }

    public T? FindNamedChunkInParentHierarchy<T>(string name) where T : NamedChunk
    {
        if (ParentFile != null)
            return ParentFile.GetFirstChunkOfType<T>(name);

        var current = ParentChunk;
        while (current != null)
        {
            var found = current.GetFirstChunkOfType<T>(name) ?? current.ParentFile?.GetFirstChunkOfType<T>(name);
            if (found != null)
                return found;

            current = current.ParentChunk;
        }

        return null;
    }

    public uint GetChildCount() => (uint)Children.Count;

    public uint GetChildCount(ChunkIdentifier chunkIdentifier) => GetChildCount((uint)chunkIdentifier);

    public uint GetChildCount(uint chunkID)
    {
        uint count = 0;
        foreach (var child in Children)
            if (child.ID == chunkID)
                count++;
        return count;
    }

    public P3DFile? GetP3DFile()
    {
        var chunk = this;
        while (chunk.ParentChunk != null)
            chunk = chunk.ParentChunk;
        return chunk.ParentFile;
    }

    /// <summary>
    /// Writes the chunk's properties to <paramref name="bw"/>.
    /// <para>This should be overwritten for known chunks.</para>
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    protected abstract void WriteData(System.IO.BinaryWriter bw);

    /// <summary>
    /// Creates a clone of the current chunk.
    /// </summary>
    /// <returns>A duplicate of the current chunk.</returns>
    protected abstract Chunk CloneSelf();

    public override string ToString() => $"{GetChunkType(this)} (0x{ID:X})";

    /// <summary>
    /// This compares the current <see cref="Chunk"/> to another.
    /// </summary>
    /// <param name="obj">The <see cref="Chunk"/> to compare to.</param>
    /// <returns>Returns <c>true</c> if <paramref name="obj"/> is a <see cref="Chunk"/>, with the same <see cref="ID"/>, <see cref="DataBytes"/> and <see cref="Children"/>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not Chunk chunk2)
            return false;

        if (ID != chunk2.ID)
            return false;

        var dataLength = DataLength;
        if (dataLength != chunk2.DataLength)
            return false;

        if (Children.Count != chunk2.Children.Count)
            return false;

        var dataBytes = DataBytes;
        var dataBytes2 = chunk2.DataBytes;
        for (int i = 0; i < dataLength; i++)
            if (dataBytes[i] != dataBytes2[i])
                return false;

        for (int i = 0; i < Children.Count; i++)
        {
            var subChunk = Children[i];
            var subChunk2 = chunk2.Children[i];

            if (!subChunk.Equals(subChunk2))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Generates a unique hash code, taking note of <see cref="ID"/>, <see cref="DataBytes"/> and <see cref="Children"/>.
    /// </summary>
    /// <returns>An <c>int</c> with the hash code.</returns>
    public override int GetHashCode()
    {
        int hash = ID.GetHashCode();
        foreach (var b in DataBytes)
            hash = (hash * 31) + b;
        foreach (var child in Children)
            hash = (hash * 31) + child.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is equal to <paramref name="right"/> using <see cref="Equals(object)"/>.
    /// </summary>
    /// <param name="left">The <see cref="Chunk"/> to compare against.</param>
    /// <param name="right">The <see cref="Chunk"/> to compare to.</param>
    /// <returns>Returns <c>true</c> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
    public static bool operator ==(Chunk? left, Chunk? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return EqualityComparer<Chunk>.Default.Equals(left, right);
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is not equal to <paramref name="right"/> using <see cref="Equals(object)"/>.
    /// </summary>
    /// <param name="left">The <see cref="Chunk"/> to compare against.</param>
    /// <param name="right">The <see cref="Chunk"/> to compare to.</param>
    /// <returns>Returns <c>true</c> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
    public static bool operator !=(Chunk? left, Chunk? right) => !(left == right);

    protected static string GetChunkType(Chunk chunk)
    {
        if (Enum.IsDefined(typeof(ChunkIdentifier), (ChunkIdentifier)chunk.ID))
            return ((ChunkIdentifier)chunk.ID).ToString().Replace("_", " ");

        var chunkType = chunk.GetType().Name;
        if (chunkType.EndsWith("Chunk"))
            chunkType = chunkType.Substring(0, chunkType.Length - 5);
        return chunkType;
    }
}

public class UnknownChunk : Chunk
{
    /// <summary>
    /// Property <c>Data</c> is the chunk's header data.
    /// </summary>
    public byte[] Data { get; set; }

    public override byte[] DataBytes => Data;

    public override uint DataLength => (uint)Data.Length;

    /// <summary>
    /// This constructor initialises an unknown chunk with the given <paramref name="chunkId"/>. It sets <see cref="Data"/> to <paramref name="data"/>.
    /// </summary>
    /// <param name="chunkId">The chunk's identifier. See <seealso cref="ChunkIdentifier"/> for known identifiers.</param>
    /// <param name="data">The chunks's data. Be aware that this must be formatted in the system's default <see cref="Endianness"/>.</param>
    public UnknownChunk(uint chunkId, byte[] data) : base(chunkId)
    {
        ID = chunkId;

        Data = (byte[])data.Clone();
    }

    protected override void WriteData(System.IO.BinaryWriter bw) => bw.Write(Data);

    protected override Chunk CloneSelf() => new UnknownChunk(ID, Data);
}