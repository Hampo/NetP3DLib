using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PackedNormalListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Packed_Normal_List;
    
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
    public List<byte> Normals { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumNormals));
            data.AddRange(Normals);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(byte) * NumNormals;

    public PackedNormalListChunk(BinaryReader br) : base(ChunkID)
    {
        var numNormals = br.ReadInt32();
        Normals.AddRange(br.ReadBytes(numNormals));
    }

    public PackedNormalListChunk(IList<byte> normals) : base(ChunkID)
    {
        Normals.AddRange(normals);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumNormals);
        bw.Write(Normals.ToArray());
    }

    internal override Chunk CloneSelf() => new PackedNormalListChunk(Normals);
}