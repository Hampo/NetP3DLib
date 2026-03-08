using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageIndexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Index_List;
    
    public uint Version { get; set; }
    public uint Param { get; set; }
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
    public SizeAwareList<ushort> Indices { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Param));
            data.AddRange(BitConverter.GetBytes(sizeof(ushort) * NumIndices));
            foreach (var index in Indices)
                data.AddRange(BitConverter.GetBytes(index));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(ushort) * NumIndices;

    public MemoryImageIndexListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadUInt32();
        int numIndices = br.ReadInt32() / sizeof(ushort);
        var indices = new ushort[numIndices];
        for (int i = 0; i < numIndices; i++)
            indices[i] = br.ReadUInt16();
        Indices = CreateSizeAwareList(indices);
    }

    public MemoryImageIndexListChunk(uint version, uint param, IList<ushort> indices) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Indices = CreateSizeAwareList(indices);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(sizeof(ushort) * NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
    }

    protected override Chunk CloneSelf() => new MemoryImageIndexListChunk(Version, Param, Indices);
}