using System.Collections.Generic;
using System.Linq;
using System;
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
    /// Property <c>Children</c> is a list of a chunk's children.
    /// </summary>
    public List<Chunk> Children { get; } = [];

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
    public uint Size => HeaderSize + (uint)Children.Sum(x => x.Size);
    /// <summary>
    /// Property <c>DataBytes</c> is the chunk's data, built from the chunk's properties.
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
    internal Chunk(uint chunkId)
    {
        ID = chunkId;
    }

    /// <summary>
    /// This constructor initialises an unknown chunk with the given <paramref name="chunkId"/>.
    /// </summary>
    /// <param name="chunkId">The chunk's identifier. See <seealso cref="ChunkIdentifier"/> for known identifiers.</param>
    internal Chunk(ChunkIdentifier chunkId) : this((uint)chunkId) { }

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

    public T GetFirstChunkOfType<T>() where T : Chunk => Children.OfType<T>()?.FirstOrDefault();

    public T GetFirstChunkOfType<T>(string name) where T : NamedChunk => Children.OfType<T>()?.FirstOrDefault(x => x.Name.Equals(name));

    public T GetFirstParamOfType<T>(string param) where T : ParamChunk => Children.OfType<T>()?.FirstOrDefault(x => x.Param.Equals(param));

    public T GetLastChunkOfType<T>() where T : Chunk => Children.OfType<T>()?.LastOrDefault();

    public T GetLastChunkOfType<T>(string name) where T : NamedChunk => Children.OfType<T>()?.LastOrDefault(x => x.Name.Equals(name));

    public T GetLastParamOfType<T>(string param) where T : ParamChunk => Children.OfType<T>()?.LastOrDefault(x => x.Param.Equals(param));

    public T[] GetChunksOfType<T>() where T : Chunk => Children.OfType<T>().ToArray();

    public T[] GetChunksOfType<T>(string name) where T : NamedChunk => Children.OfType<T>().Where(x => x.Name.Equals(name)).ToArray();

    public T[] GetParamsOfType<T>(string param) where T : ParamChunk => Children.OfType<T>().Where(x => x.Param.Equals(param)).ToArray();

    /// <summary>
    /// Writes the chunk's properties to <paramref name="bw"/>.
    /// <para>This should be overwritten for known chunks.</para>
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    internal abstract void WriteData(System.IO.BinaryWriter bw);

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

        if (DataLength != chunk2.DataLength)
            return false;

        if (Children.Count != chunk2.Children.Count)
            return false;

        if (!DataBytes.SequenceEqual(chunk2.DataBytes))
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
        int hashCode = 1251651808;
        hashCode = hashCode * -1521134295 + ID.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(DataBytes);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<Chunk>>.Default.GetHashCode(Children);
        return hashCode;
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is equal to <paramref name="right"/> using <see cref="Equals(object)"/>.
    /// </summary>
    /// <param name="left">The <see cref="Chunk"/> to compare against.</param>
    /// <param name="right">The <see cref="Chunk"/> to compare to.</param>
    /// <returns>Returns <c>true</c> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
    public static bool operator ==(Chunk left, Chunk right) => EqualityComparer<Chunk>.Default.Equals(left, right);

    /// <summary>
    /// Checks if <paramref name="left"/> is not equal to <paramref name="right"/> using <see cref="Equals(object)"/>.
    /// </summary>
    /// <param name="left">The <see cref="Chunk"/> to compare against.</param>
    /// <param name="right">The <see cref="Chunk"/> to compare to.</param>
    /// <returns>Returns <c>true</c> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
    public static bool operator !=(Chunk left, Chunk right) => !(left == right);
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

        Data = data;
    }

    internal override void WriteData(System.IO.BinaryWriter bw) => bw.Write(Data);
}