using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMeshTreeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Mesh_Tree;
    
    public uint Version { get; set; }
    public uint FirstNode { get; set; }
    public uint HierarchyIndex { get; set; }
    public Vector3 Low { get; set; }
    public Vector3 High { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(FirstNode));
            data.AddRange(BitConverter.GetBytes(HierarchyIndex));
            data.AddRange(BinaryExtensions.GetBytes(Low));
            data.AddRange(BinaryExtensions.GetBytes(High));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3;

    public CollisionMeshTreeChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        FirstNode = br.ReadUInt32();
        HierarchyIndex = br.ReadUInt32();
        Low = br.ReadVector3();
        High = br.ReadVector3();
    }

    public CollisionMeshTreeChunk(uint version, uint firstNode, uint hierarchyIndex, Vector3 low, Vector3 high) : base(ChunkID)
    {
        Version = version;
        FirstNode = firstNode;
        HierarchyIndex = hierarchyIndex;
        Low = low;
        High = high;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(FirstNode);
        bw.Write(HierarchyIndex);
        bw.Write(Low);
        bw.Write(High);
    }
}