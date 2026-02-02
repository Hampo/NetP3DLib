using NetP3DLib.Comparer;
using NetP3DLib.IO;
using NetP3DLib.P3D.Chunks;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace NetP3DLib.P3D;

public class P3DFile
{
    public const uint SIGNATURE = 0xFF443350;
    public const uint SIGNATURE_SWAP = 0x503344FF;
    public const uint COMPRESSED_SIGNATURE = 0x5A443350;
    public const uint COMPRESSED_SIGNATURE_SWAP = 0x5033445A;
    public const uint HEADER_SIZE = sizeof(uint) + sizeof(uint) + sizeof(uint); // ID + HeaderLength + ChunkLength

    public ChunkFileCollection Chunks { get; private set; }
    public ReadOnlyCollection<Chunk> AllChunks
    {
        get
        {
            var result = new List<Chunk>(Chunks.Count * 4);

            var stack = new Stack<Chunk>(Chunks.Count);
            for (int i = Chunks.Count - 1; i >= 0; i--)
                stack.Push(Chunks[i]);

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

    public uint Size => HEADER_SIZE + Chunks.TotalSize;
    public byte[] Bytes
    {
        get
        {
            List<byte> bytes = [];
            bytes.AddRange(BitConverter.GetBytes(SIGNATURE));
            bytes.AddRange(BitConverter.GetBytes(HEADER_SIZE));
            bytes.AddRange(BitConverter.GetBytes(Size));
            foreach (var chunk in Chunks)
                bytes.AddRange(chunk.Bytes);
            return [.. bytes];
        }
    }

    /// <summary>
    /// When using <see cref="SortChunks"/>, this is the chunk sort order.
    /// <para>Defaults to the order <c>Simpsons: Hit &amp; Run</c> expects.</para>
    /// </summary>
    public static readonly List<uint> ChunkSortPriority = [
        (uint)ChunkIdentifier.History,
        (uint)ChunkIdentifier.Export_Info,
        (uint)ChunkIdentifier.Tree,
        (uint)ChunkIdentifier.Image,
        (uint)ChunkIdentifier.Texture,
        (uint)ChunkIdentifier.Set,
        (uint)ChunkIdentifier.Shader,
        (uint)ChunkIdentifier.Texture_Font,
        (uint)ChunkIdentifier.Image_Font,
        (uint)ChunkIdentifier.Sprite,
        (uint)ChunkIdentifier.Old_Vertex_Anim_Key_Frame,
        (uint)ChunkIdentifier.Animation,
        (uint)ChunkIdentifier.Light,
        (uint)ChunkIdentifier.Light_Group,
        (uint)ChunkIdentifier.Photon_Map,
        (uint)ChunkIdentifier.Camera,
        (uint)ChunkIdentifier.Skeleton,
        (uint)ChunkIdentifier.Lens_Flare_Group,
        (uint)ChunkIdentifier.Particle_System_Factory,
        (uint)ChunkIdentifier.Particle_System_2,
        (uint)ChunkIdentifier.Old_Billboard_Quad,
        (uint)ChunkIdentifier.Old_Billboard_Quad_Group,
        (uint)ChunkIdentifier.Mesh,
        (uint)ChunkIdentifier.Physics_Object,
        (uint)ChunkIdentifier.Collision_Object,
        (uint)ChunkIdentifier.Anim_Obj_Wrapper,
        (uint)ChunkIdentifier.Breakable_Object,
        (uint)ChunkIdentifier.Inst_Particle_System,
        (uint)ChunkIdentifier.Anim_Dyna_Phys_Wrapper,
        (uint)ChunkIdentifier.Collision_Effect,
        (uint)ChunkIdentifier.Instance_List,
        (uint)ChunkIdentifier.Anim_Dyna_Phys,
        (uint)ChunkIdentifier.Anim_Coll,
        (uint)ChunkIdentifier.Skin,
        (uint)ChunkIdentifier.Expression_Group,
        (uint)ChunkIdentifier.Expression_Mixer,
        (uint)ChunkIdentifier.Wall,
        (uint)ChunkIdentifier.Fenceline,
        (uint)ChunkIdentifier.Fence,
        (uint)ChunkIdentifier.Intersection,
        (uint)ChunkIdentifier.Road_Data_Segment,
        (uint)ChunkIdentifier.Road_Segment,
        (uint)ChunkIdentifier.Road,
        (uint)ChunkIdentifier.Path,
        (uint)ChunkIdentifier.Anim,
        (uint)ChunkIdentifier.Locator,
        (uint)ChunkIdentifier.Spline,
        (uint)ChunkIdentifier.Composite_Drawable,
        (uint)ChunkIdentifier.Animated_Object_Factory,
        (uint)ChunkIdentifier.Animated_Object,
        (uint)ChunkIdentifier.Scenegraph,
        (uint)ChunkIdentifier.Frame_Controller,
        (uint)ChunkIdentifier.Old_Frame_Controller,
        (uint)ChunkIdentifier.Multi_Controller,
        (uint)ChunkIdentifier.Old_Locator,
        (uint)ChunkIdentifier.Game_Attr,
        (uint)ChunkIdentifier.Smart_Prop,
        (uint)ChunkIdentifier.State_Prop_Data_V1,
    ];

    public P3DFile()
    {
        Chunks = new(this);
    }

    public P3DFile (string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Could not find the specified file.", filePath);

        FileStream? fs = null;
        MemoryStream? ms = null;
        EndianAwareBinaryReader? br = null;
        try
        {
            fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < HEADER_SIZE)
                throw new InvalidDataException("Specified file too short. Must be at least 12 bytes.");

            byte[] signatureBuffer = new byte[4];
            fs.Read(signatureBuffer, 0, 4);
            uint signature = BitConverter.ToUInt32(signatureBuffer, 0);

            var endianness = (signature == SIGNATURE_SWAP || signature == COMPRESSED_SIGNATURE_SWAP) ? BinaryExtensions.SwappedEndian : BinaryExtensions.DefaultEndian;

            switch (signature)
            {
                case SIGNATURE:
                case SIGNATURE_SWAP:
                    br = new(fs, endianness);
                    break;
                case COMPRESSED_SIGNATURE:
                case COMPRESSED_SIGNATURE_SWAP:
                    fs.Position = 0;
                    using (EndianAwareBinaryReader br2 = new(fs, endianness))
                    {
                        byte[] decryptedBytes = LZR_Compression.DecompressFile(br2);
                        ms = new(decryptedBytes);
                        br = new(ms, endianness);
                        if (br.ReadUInt32() != SIGNATURE)
                            throw new InvalidDataException($"Decompressed file signature 0x{signature:X} is invalid.");
                    }
                    break;
                default:
                    throw new InvalidDataException($"File signature 0x{signature:X} is invalid.");
            }

            uint headerSize = br.ReadUInt32();
            if (headerSize != HEADER_SIZE)
                throw new InvalidDataException($"P3D File header size incorrect. Expected 12, read {headerSize}.");

            uint fileSize = br.ReadUInt32();
            if (fileSize != br.BaseStream.Length)
                throw new InvalidDataException($"P3D File full size incorrect. Expected {br.BaseStream.Length}, read {fileSize}.");

            uint bytesRead = headerSize;
            if (bytesRead < fileSize)
            {
                Chunks = new(this, (int)(fileSize - bytesRead) / 12);
                while (bytesRead < fileSize)
                {
                    Chunk c = ChunkLoader.LoadChunk(br);
                    Chunks.Add(c);
                    bytesRead += c.Size;
                }
            }
            else
            {
                Chunks = new(this);
            }
        }
        finally
        {
            br?.Close();
            br?.Dispose();
            
            ms?.Close();
            ms?.Dispose();

            fs?.Close();
            fs?.Dispose();
        }
    }

    private static readonly Regex SectionRegex = new(@"^(.*?) \(0x([A-F0-9]+)\)$");
    /// <summary>
    /// Sort <see cref="Chunks"/> in the order specified in <see cref="ChunkSortPriority"/>.
    /// <para>Chunk IDs without a sort priority will remain in their original order at the end of the file.</para>
    /// </summary>
    /// <param name="includeSectionHeaders">If <c>true</c>, will add a History chunk before each chunk ID.</param>
    /// <param name="alphabetical">If <c>true</c>, named chunks will be further sorted into alphabetical order.</param>
    /// <param name="includeHistory">If <c>true</c>, a history chunk will be added to the start to indicate how and when it was sorted.<para>Defaults to <c>true</c>.</para></param>
    /// <param name="caseInsensitive">If <c>true</c>, and <paramref name="alphabetical"/> is <c>true</c>, named chunk sorting will be case insensitive</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0220:Add explicit cast", Justification = "As the list is all chunks of the same type, if the first is a LocatorChunk, the rest will be.")]
    public void SortChunks(bool includeSectionHeaders = false, bool alphabetical = false, bool includeHistory = true, bool caseInsensitive = false)
    {
        List<Chunk> newChunks = new(Chunks.Count);

        if (includeHistory)
            Chunks.Add(new HistoryChunk(["Sorted with NetP3DLib", $"Run at {DateTime.Now:R}"]));

        var comparer = new NaturalComparer(caseInsensitive);//caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        Dictionary<uint, List<Chunk>> chunksById = [];
        foreach (var chunk in Chunks)
        {
            if (!chunksById.TryGetValue(chunk.ID, out var list))
            {
                list = [];
                chunksById[chunk.ID] = list;
            }
            list.Add(chunk);
        }

        List<uint> chunkIDs = [.. chunksById.Keys];
        chunkIDs.Sort((a, b) =>
        {
            int indexA = ChunkSortPriority.IndexOf(a);
            int indexB = ChunkSortPriority.IndexOf(b);

            bool aInList = indexA >= 0;
            bool bInList = indexB >= 0;

            if (aInList && bInList)
                return indexA.CompareTo(indexB);

            if (aInList)
                return -1;

            if (bInList)
                return 1;

            return a.CompareTo(b);
        });

        foreach (var id in chunkIDs)
        {
            var chunks = chunksById[id];

            if (chunks[0] is HistoryChunk)
            {
                for (int i = chunks.Count - 1; i >= 0; i--)
                {
                    var historyChunk = (HistoryChunk)chunks[i];

                    if (historyChunk.NumHistory != 1)
                        continue;

                    var sectionMatch = SectionRegex.Match(historyChunk.History[0]);
                    if (!sectionMatch.Success)
                        continue;

                    var typeName = sectionMatch.Groups[1].Value;
                    var typeID = int.Parse(sectionMatch.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                    var typeIdentifier = (ChunkIdentifier)typeID;

                    if ((typeIdentifier == ChunkIdentifier.Locator && typeName.Contains("Locator")) || typeIdentifier.ToString().Replace("_", " ") == typeName)
                        chunks.RemoveAt(i);
                }
            }

            if (chunks.Count == 0)
                continue;

            if (chunks[0] is LocatorChunk)
            {
                Dictionary<uint, List<LocatorChunk>> locatorChunksByType = [];
                foreach (LocatorChunk locatorChunk in chunks)
                {
                    if (!locatorChunksByType.TryGetValue((uint)locatorChunk.LocatorType, out var locatorList))
                    {
                        locatorList = [];
                        locatorChunksByType[(uint)locatorChunk.LocatorType] = locatorList;
                    }
                    locatorList.Add(locatorChunk);
                }

                List<uint> locatorTypes = [.. locatorChunksByType.Keys];
                locatorTypes.Sort();

                foreach (uint type in locatorTypes)
                {
                    var typeChunks = locatorChunksByType[type];

                    if (includeSectionHeaders)
                        newChunks.Add(new HistoryChunk([$"{typeChunks[0].LocatorType} Locator (Type {type}) (0x{id:X})"]));

                    if (alphabetical)
                        typeChunks.Sort((x, y) => comparer.Compare(x.Name, y.Name));

                    newChunks.AddRange(typeChunks);
                }

                continue;
            }

            if (includeSectionHeaders)
                newChunks.Add(new HistoryChunk([$"{((ChunkIdentifier)id).ToString().Replace("_", " ")} (0x{id:X})"]));

            if (alphabetical && chunks[0] is NamedChunk)
            {
                List<NamedChunk> namedChunks = new(chunks.Count);
                foreach (var chunk in chunks)
                    namedChunks.Add((NamedChunk)chunk);

                namedChunks.Sort((x, y) => comparer.Compare(x.Name, y.Name));
                newChunks.AddRange(namedChunks);
            }
            else
            {
                newChunks.AddRange(chunks);
            }
        }

        Chunks.Clear();
        Chunks.AddRange(newChunks);
    }

    public T? GetFirstChunkOfType<T>() where T : Chunk
    {
        foreach (var child in Chunks)
            if (child is T chunk)
                return chunk;
        return null;
    }

    public T? GetFirstChunkOfType<T>(string name) where T : NamedChunk
    {
        foreach (var child in Chunks)
            if (child is T chunk && chunk.Name == name)
                return chunk;
        return null;
    }

    public T? GetFirstParamOfType<T>(string param) where T : ParamChunk
    {
        foreach (var child in Chunks)
            if (child is T chunk && chunk.Param == param)
                return chunk;
        return null;
    }

    public T? GetLastChunkOfType<T>() where T : Chunk
    {
        for (int i = Chunks.Count - 1; i >= 0; i--)
            if (Chunks[i] is T chunk)
                return chunk;
        return null;
    }

    public T? GetLastChunkOfType<T>(string name) where T : NamedChunk
    {
        for (int i = Chunks.Count - 1; i >= 0; i--)
            if (Chunks[i] is T chunk && chunk.Name == name)
                return chunk;
        return null;
    }

    public T? GetLastParamOfType<T>(string param) where T : ParamChunk
    {
        for (int i = Chunks.Count - 1; i >= 0; i--)
            if (Chunks[i] is T chunk && chunk.Param == param)
                return chunk;
        return null;
    }

    public IReadOnlyList<T> GetChunksOfType<T>() where T : Chunk
    {
        var result = new List<T>();
        foreach (var child in Chunks)
        {
            if (child is T chunk)
                result.Add(chunk);
        }
        return result;
    }

    public IReadOnlyList<T> GetChunksOfType<T>(string name) where T : NamedChunk
    {
        var result = new List<T>();
        foreach (var child in Chunks)
        {
            if (child is T chunk && chunk.Name == name)
                result.Add(chunk);
        }
        return result;
    }

    public IReadOnlyList<T> GetParamsOfType<T>(string param) where T : ParamChunk
    {
        var result = new List<T>();
        foreach (var child in Chunks)
        {
            if (child is T chunk && chunk.Param == param)
                result.Add(chunk);
        }
        return result;
    }

    public P3DFile Clone()
    {
        var clone = new P3DFile();
        foreach (var child in Chunks)
            clone.Chunks.Add(child.Clone());
        return clone;
    }

    public void Write(string filePath) => Write(filePath, BinaryExtensions.DefaultEndian, true);

    public void Write(string filePath, bool validate) => Write(filePath, BinaryExtensions.DefaultEndian, validate);

    public void Write(string filePath, Endianness endianness) => Write(filePath, endianness, true);

    public void Write(string filePath, Endianness endianness, bool validate)
    {
        using FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        Write(fs, endianness, validate);
    }

    public void Write(Stream stream) => Write(stream, BinaryExtensions.DefaultEndian, true);

    public void Write(Stream stream, bool validate) => Write(stream, BinaryExtensions.DefaultEndian, validate);

    public void Write(Stream stream, Endianness endianness) => Write(stream, endianness, true);

    public void Write(Stream stream, Endianness endianness, bool validate)
    {
        if (!stream.CanWrite)
            throw new ArgumentException("Cannot write to stream.", nameof(stream));

        if (validate)
            foreach (var c in Chunks)
                c.Validate();

        EndianAwareBinaryWriter bw = new(stream, endianness);

        bw.Write(SIGNATURE);
        bw.Write(HEADER_SIZE);
        bw.Write(Size);
        foreach (Chunk chunk in Chunks)
            chunk.Write(bw);
        bw.Flush();
    }

    public void Compress(string filePath, bool includeHistory = true, bool fast = false, bool validate = true)
    {
        using FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        Compress(fs, includeHistory, fast, validate);
    }

    public void Compress(Stream stream, bool includeHistory = true, bool fast = false, bool validate = true)
    {
        if (!stream.CanWrite)
            throw new ArgumentException("Cannot write to stream.", nameof(stream));

        var compressedBytes = LZR_Compression.CompressFile(this, includeHistory, fast, validate);
        stream.Write(compressedBytes, 0, compressedBytes.Length);
        stream.Flush();
    }

    public override string ToString()
    {
        int totalChunks = Chunks.Count;
        int totalSubChunks = AllChunks.Count - totalChunks;

        return $"P3DFile: {totalChunks:N0} root chunk{(totalChunks != 1 ? "s" : "")}; {totalSubChunks:N0} subchunk{(totalSubChunks != 1 ? "s" : "")}; {Size:N0} bytes";
    }
}
