using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;

#if DEBUG
using NetP3DLib.P3D.Enums;
#endif
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace NetP3DLib.P3D;

public static class ChunkLoader
{
    public static readonly Dictionary<uint, (Type, Func<BinaryReader, Chunk>)> ChunkTypes;
#if DEBUG
    public static HashSet<uint> UnknownChunks = [];
#endif

    static ChunkLoader()
    {
        ChunkTypes = [];

#if !DEBUG
        LoadChunkTypes("NetP3DLib.P3D.Chunks", false, false);
        LoadChunkTypes("NetP3DLib.P3D.UnknownChunks", false, false);
#else
        LoadChunkTypes("NetP3DLib.P3D.Chunks", true, true);
        LoadChunkTypes("NetP3DLib.P3D.UnknownChunks", true, true);
        Console.WriteLine($"Known chunk identifiers: {Enum.GetValues(typeof(ChunkIdentifier)).Length}. Loaded chunk classes: {ChunkTypes.Count}.");
        foreach (var chunkIdentifier in Enum.GetValues(typeof(ChunkIdentifier)))
            if (!ChunkTypes.ContainsKey((uint)chunkIdentifier))
                Console.WriteLine($"\tMissing class for identifier: {chunkIdentifier} (0x{(uint)chunkIdentifier:X})");
#endif
    }

    /// <summary>
    /// Loads chunk definitions from the given <paramref name="nameSpace"/>.
    /// <para>If <paramref name="throwOnDuplicate"/> is <c>true</c>, <see cref="InvalidOperationException"/> will be thrown if there is already a chunk with a matching identifier loaded.</para>
    /// <para>if <paramref name="throwOnInvalid"/> is <c>true</c>, <see cref="InvalidOperationException"/> will be thrown if a chunk class in the namespace is invalid.</para>
    /// </summary>
    /// <param name="nameSpace">The namespace to load chunk definitions from.</param>
    /// <param name="throwOnDuplicate">If <c>true</c> duplicate chunks will error. If <c>false</c> duplicate chunks will overwrite.</param>
    /// <param name="throwOnInvalid">If <c>true</c> invalid chunks will error. If <c>false</c> invalid chunks will be ignored.</param>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="nameSpace"/> is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown is <paramref name="throwOnDuplicate"/> or <paramref name="throwOnInvalid"/> are <c>true</c> and an invalid chunk is encountered.</exception>
    public static void LoadChunkTypes(string nameSpace, bool throwOnDuplicate = true, bool throwOnInvalid = true)
    {
        if (string.IsNullOrWhiteSpace(nameSpace))
            throw new ArgumentException($"{nameof(nameSpace)} cannot be null.", nameof(nameSpace));

        var chunkTypeList = GetTypesInNamespace(Assembly.GetExecutingAssembly(), nameSpace);
        Type baseType = typeof(Chunk);

        foreach (Type chunkType in chunkTypeList)
        {
            if (chunkType.IsNested)
                continue;

            if (!chunkType.IsSubclassOf(baseType))
            {
                if (throwOnInvalid)
                    throw new InvalidOperationException($"Type {chunkType.FullName} is not a subclass of {baseType.FullName}.");

#if DEBUG
                Console.WriteLine($"Type {chunkType.FullName} is not a subclass of {baseType.Namespace}. Skipping.");
#endif
                continue;
            }

            ChunkAttributes chunkAttriutes = chunkType.GetCustomAttribute<ChunkAttributes>(false);
            if (chunkAttriutes == null)
            {
                if (throwOnInvalid)
                    throw new InvalidOperationException($"Type {chunkType.FullName} does not have a `ChunkAttributes` attribute.");

#if DEBUG
                Console.WriteLine($"Type {chunkType.FullName} does not have a `ChunkAttributes` attribute. Skipping.");
#endif
                continue;
            }

            if (throwOnDuplicate && ChunkTypes.ContainsKey(chunkAttriutes.Identifier))
                throw new InvalidOperationException($"Chunk type with identifier 0x{chunkAttriutes.Identifier:X} already exists.");


            var ctor = chunkType.GetConstructor([typeof(BinaryReader)]);
            if (ctor == null)
            {
                if (throwOnInvalid)
                    throw new InvalidOperationException(
                        $"Type {chunkType.FullName} must have a constructor with a BinaryReader parameter.");

#if DEBUG
                Console.WriteLine($"[ChunkLoader] Skipped: {chunkType.FullName} lacks expected constructor.");
#endif
                continue;
            }

            var param = Expression.Parameter(typeof(BinaryReader), "br");
            var newExpr = Expression.New(ctor, param);
            var lambda = Expression.Lambda<Func<BinaryReader, Chunk>>(newExpr, param);
            ChunkTypes[chunkAttriutes.Identifier] = (chunkType, lambda.Compile());
        }
    }

    /// <summary>
    /// Loads a chunk from a <see cref="BinaryReader"/>. Assumes the stream position is at the start of the chunk.
    /// </summary>
    /// <param name="br">The <see cref="BinaryReader"/> to read from. <see cref="EndianAwareBinaryReader"/> is preferred.</param>
    /// <returns>A known chunk class if one exists in <see cref="ChunkTypes"/>, or <see cref="UnknownChunk"/> otherwise.</returns>
    public static Chunk LoadChunk(BinaryReader br)
    {
        uint chunkId = br.ReadUInt32();
        uint headerSize = br.ReadUInt32() - P3DFile.HEADER_SIZE;
        uint chunkSize = br.ReadUInt32();

        byte[] headerData = br.ReadBytes((int)headerSize);

        Chunk c;

        uint actualHeaderSize = headerSize;
        if (ChunkTypes.TryGetValue(chunkId, out var entry))
        {
            var (chunkType, chunkConstructor) = entry;

            Endianness endianness;
            if (br is EndianAwareBinaryReader eabr)
                endianness = eabr.Endianness;
            else
                endianness = BinaryExtensions.DefaultEndian;

            using MemoryStream ms = new(headerData);
            using EndianAwareBinaryReader br2 = new(ms, endianness);
            c = chunkConstructor(br2);
            actualHeaderSize = (uint)ms.Position;
        }
        else
        {
            c = new UnknownChunk(chunkId, headerData);
#if DEBUG
            UnknownChunks.Add(chunkId);
#endif
        }

        if (headerSize != actualHeaderSize)
        {
            var diff = headerSize - actualHeaderSize;
#if DEBUG
            if (chunkId != (uint)ChunkIdentifier.Physics_Object && chunkId != (uint)ChunkIdentifier.Animation_Header)
                Debugger.Break();
#endif
            br.BaseStream.Position -= diff;
        }

        uint bytesRead = actualHeaderSize + P3DFile.HEADER_SIZE;
        if (bytesRead < chunkSize)
        {
            c.Children = new((int)(chunkSize - bytesRead) / 12);
            while (bytesRead < chunkSize)
            {
                Chunk subChunk = LoadChunk(br);
                c.Children.Add(subChunk);
                bytesRead += subChunk.Size;
            }
        }
            
        return c;
    }

    private static IReadOnlyList<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
    {
        var types = new List<Type>();
        try
        {
            foreach (var type in assembly.GetTypes())
                if (type?.Namespace == nameSpace)
                    types.Add(type);
        }
        catch (ReflectionTypeLoadException ex)
        {
            var loaderExceptions = ex.LoaderExceptions;
            foreach (var loaderException in loaderExceptions)
                Console.WriteLine(loaderException);

            foreach (var type in ex.Types)
                if (type?.Namespace == nameSpace)
                    types.Add(type);
        }
        return types;
    }
}
