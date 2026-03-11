using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageVertexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Vertex_List;

    public uint Version { get; set; }
    public uint Param { get; set; }
    public uint VertexSize
    {
        get => (uint)(Vertex?.Count ?? 0);
        set
        {
            if (value == VertexSize)
                return;

            if (value < VertexSize)
            {
                while (VertexSize > value)
                    Vertex.RemoveAt(Vertex.Count - 1);
            }
            else
            {
                while (VertexSize < value)
                    Vertex.Add(default);
            }
        }
    }
    public SizeAwareList<byte> Vertex { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Param));
            data.AddRange(BitConverter.GetBytes(VertexSize));
            data.AddRange(Vertex);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + VertexSize;

    public MemoryImageVertexListChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadUInt32(), ListHelper.ReadArray(br.ReadInt32(), br.ReadByte))
    {
    }

    public MemoryImageVertexListChunk(uint version, uint param, IList<byte> vertex) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Vertex = CreateSizeAwareList(vertex);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(VertexSize);
        bw.Write([.. Vertex]);
    }

    protected override Chunk CloneSelf() => new MemoryImageVertexListChunk(Version, Param, Vertex);
}