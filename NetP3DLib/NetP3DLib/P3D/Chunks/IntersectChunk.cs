using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IntersectChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Intersect;

    public uint NumIndices
    {
        get => (uint)(Indices?.Count ?? 0);
        set
        {
            if (value == NumIndices)
                return;

            if (value < NumIndices)
            {
                while (NumIndices > value)
                    Indices.RemoveAt(Indices.Count - 1);
            }
            else
            {
                while (NumIndices < value)
                    Indices.Add(default);
            }
        }
    }
    public SizeAwareList<uint> Indices { get; }
    public uint NumPositions
    {
        get => (uint)(Positions?.Count ?? 0);
        set
        {
            if (value == NumPositions)
                return;

            if (value < NumPositions)
            {
                while (NumPositions > value)
                    Positions.RemoveAt(Positions.Count - 1);
            }
            else
            {
                while (NumPositions < value)
                    Positions.Add(default);
            }
        }
    }
    public SizeAwareList<Vector3> Positions { get; }
    public uint NumNormals
    {
        get => (uint)(Normals?.Count ?? 0);
        set
        {
            if (value == NumNormals)
                return;

            if (value < NumNormals)
            {
                while (NumNormals > value)
                    Normals.RemoveAt(Normals.Count - 1);
            }
            else
            {
                while (NumNormals < value)
                    Normals.Add(default);
            }
        }
    }
    public SizeAwareList<Vector3> Normals { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumIndices));
            foreach (var i in Indices)
                data.AddRange(BitConverter.GetBytes(i));
            data.AddRange(BitConverter.GetBytes(NumPositions));
            foreach (var pos in Positions)
                data.AddRange(BinaryExtensions.GetBytes(pos));
            data.AddRange(BitConverter.GetBytes(NumNormals));
            foreach (var normal in Normals)
                data.AddRange(BinaryExtensions.GetBytes(normal));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumIndices + sizeof(uint) + sizeof(float) * 3 * NumPositions + sizeof(uint) + sizeof(float) * 3 * NumNormals;

    public IntersectChunk(EndianAwareBinaryReader br) : this(ListHelper.ReadArray(br.ReadInt32(), br.ReadUInt32), ListHelper.ReadArray(br.ReadInt32(), br.ReadVector3), ListHelper.ReadArray(br.ReadInt32(), br.ReadVector3))
    {
    }

    public IntersectChunk(IList<uint> indices, IList<Vector3> positions, IList<Vector3> normals) : base(ChunkID)
    {
        Indices = CreateSizeAwareList(indices);
        Positions = CreateSizeAwareList(positions);
        Normals = CreateSizeAwareList(normals);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Indices.Count % 3 != 0)
            yield return new InvalidP3DException(this, $"The number of {nameof(Indices)} must be divisible by 3.");

        for (var i = 0; i < Indices.Count; i++)
            if (Indices[i] >= Positions.Count)
                yield return new InvalidP3DException(this, $"The {nameof(Indices)} at index {i} is out of bound of the {nameof(Positions)} list.");

        var numTriangles = Indices.Count / 3;
        if (Normals.Count != numTriangles)
            yield return new InvalidP3DException(this, $"The number of {nameof(Normals)} does not match the number of triangles ({numTriangles}).");

        var terrainTypeListChunk = GetFirstChunkOfType<TerrainTypeListChunk>();
        if (terrainTypeListChunk != null && terrainTypeListChunk.NumTypes != NumNormals)
            yield return new InvalidP3DException(this, $"The number of {nameof(Normals)} does not match the number of terrain types ({terrainTypeListChunk.NumTypes}).");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumIndices);
        foreach (var i in Indices)
            bw.Write(i);
        bw.Write(NumPositions);
        foreach (var pos in Positions)
            bw.Write(pos);
        bw.Write(NumNormals);
        foreach (var normal in Normals)
            bw.Write(normal);
    }

    protected override Chunk CloneSelf() => new IntersectChunk(Indices, Positions, Normals);
}