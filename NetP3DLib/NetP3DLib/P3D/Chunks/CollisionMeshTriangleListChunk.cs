using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMeshTriangleListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Mesh_Triangle_list;
    
    public uint NumTriangles
    {
        get => (uint)Triangles.Count;
        set
        {
            if (value == NumTriangles)
                return;

            if (value < NumTriangles)
            {
                while (NumTriangles > value)
                    Triangles.RemoveAt(Triangles.Count - 1);
            }
            else
            {
                while (NumTriangles < value)
                    Triangles.Add(new());
            }
        }
    }
    public List<Triangle> Triangles { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumTriangles));
            foreach (var entry in Triangles)
                data.AddRange(entry.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + Triangle.Size * NumTriangles;

    public CollisionMeshTriangleListChunk(BinaryReader br) : base(ChunkID)
    {
        var numEntries = br.ReadInt32();
        Triangles = new(numEntries);
        for (int i = 0; i < numEntries; i++)
            Triangles.Add(new(br));
    }

    public CollisionMeshTriangleListChunk(IList<Triangle> entries) : base(ChunkID)
    {
        Triangles.AddRange(entries);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumTriangles);
        foreach (var entry in Triangles)
            entry.Write(bw);
    }

    internal override Chunk CloneSelf()
    {
        var triangles = new List<Triangle>(Triangles.Count);
        foreach (var triangle in Triangles)
            triangles.Add(triangle.Clone());
        return new CollisionMeshTriangleListChunk(triangles);
    }

    public class Triangle
    {
        public const uint Size = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        public ushort Index0 { get; set; }
        public ushort Index1 { get; set; }
        public ushort Index2 { get; set; }
        public ushort Flags { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BitConverter.GetBytes(Index0));
                data.AddRange(BitConverter.GetBytes(Index1));
                data.AddRange(BitConverter.GetBytes(Index2));
                data.AddRange(BitConverter.GetBytes(Flags));

                return [.. data];
            }
        }

        public Triangle(BinaryReader br)
        {
            Index0 = br.ReadUInt16();
            Index1 = br.ReadUInt16();
            Index2 = br.ReadUInt16();
            Flags = br.ReadUInt16();
        }

        public Triangle(ushort index0, ushort index1, ushort index2, ushort flags)
        {
            Index0 = index0;
            Index1 = index1;
            Index2 = index2;
            Flags = flags;
        }

        public Triangle()
        {
            Index0 = 0;
            Index1 = 0;
            Index2 = 0;
            Flags = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Index0);
            bw.Write(Index1);
            bw.Write(Index2);
            bw.Write(Flags);
        }

        internal Triangle Clone() => new(Index0, Index1, Index2, Flags);

        public override string ToString() => $"{Index0} | {Index1} | {Index2} | {Flags}";
    }
}