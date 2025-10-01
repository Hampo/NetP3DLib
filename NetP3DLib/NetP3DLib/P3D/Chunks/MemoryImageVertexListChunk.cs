using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageVertexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Vertex_List;
    
    public uint Version { get; set; }
    public uint Param { get; set; }
    public uint VertexSize
    {
        get => (uint)Vertex.Count;
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
    public List<byte> Vertex { get; } = [];

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

    public MemoryImageVertexListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadUInt32();
        int numVertices = br.ReadInt32();
        Vertex = new(numVertices);
        for (int i = 0; i < numVertices; i++)
            Vertex.Add(br.ReadByte());
    }

    public MemoryImageVertexListChunk(uint version, uint param, IList<byte> vertex) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Vertex.AddRange(vertex);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(VertexSize);
        bw.Write(Vertex.ToArray());
    }

    protected override Chunk CloneSelf() => new MemoryImageVertexListChunk(Version, Param, Vertex);
}