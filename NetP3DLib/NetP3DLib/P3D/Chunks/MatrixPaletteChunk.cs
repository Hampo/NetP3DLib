using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MatrixPaletteChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Matrix_Palette;
    
    public uint NumMatrices
    {
        get => (uint)Matrices.Count;
        set
        {
            if (value == NumMatrices)
                return;

            if (value < NumMatrices)
            {
                while (NumMatrices > value)
                    Matrices.RemoveAt(Matrices.Count - 1);
            }
            else
            {
                while (NumMatrices < value)
                    Matrices.Add(default);
            }
        }
    }
    public List<uint> Matrices { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumMatrices));
            foreach (var mat in Matrices)
                data.AddRange(BitConverter.GetBytes(mat));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumMatrices;

    public MatrixPaletteChunk(BinaryReader br) : base(ChunkID)
    {
        var numMatrices = br.ReadInt32();
        Matrices.Capacity = numMatrices;
        for (int i = 0; i < numMatrices; i++)
            Matrices.Add(br.ReadUInt32());
    }

    public MatrixPaletteChunk(IList<uint> matrices) : base(ChunkID)
    {
        Matrices.AddRange(matrices);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumMatrices);
        foreach (var mat in Matrices)
            bw.Write(mat);
    }
}