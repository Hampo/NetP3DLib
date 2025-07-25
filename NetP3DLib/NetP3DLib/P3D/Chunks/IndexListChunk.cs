using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IndexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Index_List;
    
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

    public IndexListChunk(BinaryReader br) : base(ChunkID)
    {
        var num = br.ReadInt32();
        Indices = new(num);
        for (int i = 0; i < num; i++)
            Indices.Add(br.ReadUInt32());
    }

    public IndexListChunk(IList<uint> indices) : base(ChunkID)
    {
        Indices.AddRange(indices);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
    }

    internal override Chunk CloneSelf() => new IndexListChunk(Indices);
}