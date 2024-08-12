using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Matrix_List)]
public class MatrixListChunk : Chunk
{
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
    public List<Color> Matrices { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumMatrices));
            foreach (var mat in Matrices)
                data.AddRange(BinaryExtensions.GetBytes(mat));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumMatrices;

    public MatrixListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Matrix_List)
    {
        var numMatrices = br.ReadInt32();
        Matrices.Capacity = numMatrices;
        for (int i = 0; i < numMatrices; i++)
            Matrices.Add(br.ReadColor());
    }

    public MatrixListChunk(IList<Color> matrices) : base((uint)ChunkIdentifier.Matrix_List)
    {
        Matrices.AddRange(matrices);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumMatrices);
        foreach (var mat in Matrices)
            bw.Write(mat);
    }
}