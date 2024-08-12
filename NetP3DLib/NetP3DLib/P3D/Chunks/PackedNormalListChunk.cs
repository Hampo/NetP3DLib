using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Packed_Normal_List)]
public class PackedNormalListChunk : Chunk
{
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

    public PackedNormalListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Packed_Normal_List)
    {
        var numNormals = br.ReadInt32();
        Normals.AddRange(br.ReadBytes(numNormals));
    }

    public PackedNormalListChunk(IList<byte> normals) : base((uint)ChunkIdentifier.Packed_Normal_List)
    {
        Normals.AddRange(normals);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumNormals);
        bw.Write(Normals.ToArray());
    }
}