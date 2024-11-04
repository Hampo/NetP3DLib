using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Memory_Image_Index_List)]
public class MemoryImageIndexListChunk : Chunk
{
    public uint Version { get; set; }
    public uint Param { get; set; }
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
    public List<ushort> Indices { get; } = [];

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

    public MemoryImageIndexListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Memory_Image_Index_List)
    {
        Version = br.ReadUInt32();
        Param = br.ReadUInt32();
        int numIndices = br.ReadInt32() / sizeof(ushort);
        Indices.Capacity = numIndices;
        for (int i = 0; i < numIndices; i++)
            Indices.Add(br.ReadUInt16());
    }

    public MemoryImageIndexListChunk(uint version, uint param, IList<ushort> indices) : base((uint)ChunkIdentifier.Memory_Image_Index_List)
    {
        Version = version;
        Param = param;
        Indices.AddRange(indices);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(sizeof(ushort) * NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
    }
}