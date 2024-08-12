using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Intersect)]
public class IntersectChunk : Chunk
{
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

    public IntersectChunk(BinaryReader br) : base((uint)ChunkIdentifier.Intersect)
    {
        var numIndices = br.ReadInt32();
        Indices.Capacity = numIndices;
        for (var i = 0; i < numIndices; i++)
            Indices.Add(br.ReadUInt32());
        var numPositions = br.ReadInt32();
        Positions.Capacity = numPositions;
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
        var numNormals = br.ReadInt32();
        Normals.Capacity = numNormals;
        for (var i = 0; i < numNormals; i++)
            Normals.Add(br.ReadVector3());
    }

    public IntersectChunk(IList<uint> indices, IList<Vector3> positions, IList<Vector3> normals) : base((uint)ChunkIdentifier.Intersect)
    {
        Indices.AddRange(indices);
        Positions.AddRange(positions);
        Normals.AddRange(normals);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
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
}