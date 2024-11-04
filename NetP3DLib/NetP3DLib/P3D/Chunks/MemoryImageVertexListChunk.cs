using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageVertexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Vertex_List;
    
    public uint Version { get; set; }
    public uint Param { get; set; }
    public uint NumVertices
    {
        get => (uint)Vertices.Count;
        set
        {
            if (value == NumVertices)
                return;

            if (value < NumVertices)
            {
                while (NumVertices > value)
                    Vertices.RemoveAt(Vertices.Count - 1);
            }
            else
            {
                while (NumVertices < value)
                    Vertices.Add(default);
            }
        }
    }
    public List<Vertex> Vertices { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Param));
            data.AddRange(BitConverter.GetBytes(NumVertices * Vertex.Size));
            foreach (var vertex in Vertices)
                data.AddRange(vertex.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + NumVertices * Vertex.Size;

    public MemoryImageVertexListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadUInt32();
        int numVertices = (int)(br.ReadInt32() / Vertex.Size);
        Vertices.Capacity = numVertices;
        for (int i = 0; i < numVertices; i++)
            Vertices.Add(new(br));
    }

    public MemoryImageVertexListChunk(uint version, uint param, IList<Vertex> vertices) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Vertices.AddRange(vertices);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(NumVertices * Vertex.Size);
        foreach (var vertex in Vertices)
            vertex.Write(bw);
    }

    public class Vertex
    {
        internal const uint Size = sizeof(float) * 3 + sizeof(float) * 3;
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BinaryExtensions.GetBytes(Position));
                data.AddRange(BinaryExtensions.GetBytes(Normal));

                return [.. data];
            }
        }

        public Vertex(BinaryReader br)
        {
            Position = br.ReadVector3();
            Normal = br.ReadVector3();
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        public Vertex()
        {
            Position = new();
            Normal = new();
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Position);
            bw.Write(Normal);
        }

        public override string ToString()
        {
            return $"{Position} | {Normal}";
        }
    }
}