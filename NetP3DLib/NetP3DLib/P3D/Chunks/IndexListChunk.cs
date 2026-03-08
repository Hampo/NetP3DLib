using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IndexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Index_List;

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

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumIndices));
            foreach (var index in Indices)
                data.AddRange(BitConverter.GetBytes(index));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumIndices;

    public IndexListChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        var num = br.ReadInt32();
        var indices = new uint[num];
        for (int i = 0; i < num; i++)
            indices[i] = br.ReadUInt32();
        Indices = CreateSizeAwareList(indices);
    }

    public IndexListChunk(IList<uint> indices) : base(ChunkID)
    {
        Indices = CreateSizeAwareList(indices);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
    }

    protected override Chunk CloneSelf() => new IndexListChunk(Indices);
}