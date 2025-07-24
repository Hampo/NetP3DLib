using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class NormalListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Normal_List;
    
    public uint NumNormals
    {
        get => (uint)Normals.Count;
        set
        {
            if (value == NumNormals)
                return;

            if (value < NumNormals)
            {
                while (NumNormals > value)
                    Normals.RemoveAt(Normals.Count - 1);
            }
            else
            {
                while (NumNormals < value)
                    Normals.Add(default);
            }
        }
    }
    public List<Vector3> Normals { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumNormals));
            foreach (var normal in Normals)
                data.AddRange(BinaryExtensions.GetBytes(normal));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumNormals;

    public NormalListChunk(BinaryReader br) : base(ChunkID)
    {
        var numNormals = br.ReadInt32();
        Normals = new(numNormals);
        for (var i = 0; i < numNormals; i++)
            Normals.Add(br.ReadVector3());
    }

    public NormalListChunk(IList<Vector3> normals) : base(ChunkID)
    {
        Normals.AddRange(normals);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumNormals);
        foreach (var pos in Normals)
            bw.Write(pos);
    }

    internal override Chunk CloneSelf() => new NormalListChunk(Normals);
}