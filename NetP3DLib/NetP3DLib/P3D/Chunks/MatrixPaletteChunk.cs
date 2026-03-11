using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MatrixPaletteChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Matrix_Palette;

    public uint NumMatrices
    {
        get => (uint)(Matrices?.Count ?? 0);
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
    public SizeAwareList<uint> Matrices { get; }

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

    public MatrixPaletteChunk(EndianAwareBinaryReader br) : this(ListHelper.ReadArray(br.ReadInt32(), br.ReadUInt32))
    {
    }

    public MatrixPaletteChunk(IList<uint> matrices) : base(ChunkID)
    {
        Matrices = CreateSizeAwareList(matrices);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumMatrices);
        foreach (var mat in Matrices)
            bw.Write(mat);
    }

    protected override Chunk CloneSelf() => new MatrixPaletteChunk(Matrices);
}