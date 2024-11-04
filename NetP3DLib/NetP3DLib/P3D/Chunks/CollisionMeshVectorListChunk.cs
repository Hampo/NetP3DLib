using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMeshVectorListChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Collision_Mesh_Vector_list;
    
    public uint NumVectors
    {
        get => (uint)Vectors.Count;
        set
        {
            if (value == NumVectors)
                return;

            if (value < NumVectors)
            {
                while (NumVectors > value)
                    Vectors.RemoveAt(Vectors.Count - 1);
            }
            else
            {
                while (NumVectors < value)
                    Vectors.Add(default);
            }
        }
    }
    public List<Vector3> Vectors { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumVectors));
            foreach (var uv in Vectors)
                data.AddRange(BinaryExtensions.GetBytes(uv));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumVectors;

    public CollisionMeshVectorListChunk(BinaryReader br) : base(ChunkID)
    {
        var numVectors = br.ReadInt32();
        Vectors.Capacity = numVectors;
        for (int i = 0; i < numVectors; i++)
            Vectors.Add(br.ReadVector3());
    }

    public CollisionMeshVectorListChunk(IList<Vector3> vectors) : base(ChunkID)
    {
        Vectors.AddRange(vectors);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumVectors);
        foreach (var pos in Vectors)
            bw.Write(pos);
    }
}