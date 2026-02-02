using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IntersectChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Intersect;
    
    public uint NumIndices
    {
        get => (uint)Indices.Count;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<uint> Indices { get; } = [];
    public uint NumPositions
    {
        get => (uint)Positions.Count;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<Vector3> Positions { get; } = [];
    public uint NumNormals
    {
        get => (uint)Normals.Count;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<Vector3> Normals { get; } = [];

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

    public IntersectChunk(BinaryReader br) : base(ChunkID)
    {
        var numIndices = br.ReadInt32();
        Indices = new(numIndices);
        for (var i = 0; i < numIndices; i++)
            Indices.Add(br.ReadUInt32());
        var numPositions = br.ReadInt32();
        Positions = new(numPositions);
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
        var numNormals = br.ReadInt32();
        Normals = new(numNormals);
        for (var i = 0; i < numNormals; i++)
            Normals.Add(br.ReadVector3());
    }

    public IntersectChunk(IList<uint> indices, IList<Vector3> positions, IList<Vector3> normals) : base(ChunkID)
    {
        Indices.AddRange(indices);
        Positions.AddRange(positions);
        Normals.AddRange(normals);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
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

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
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