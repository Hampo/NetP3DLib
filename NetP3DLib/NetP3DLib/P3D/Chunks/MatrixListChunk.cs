using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MatrixListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Matrix_List;

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
                    Matrices.Add(new());
            }
        }
    }
    public SizeAwareList<Matrix> Matrices { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumMatrices));
            foreach (var mat in Matrices)
                data.AddRange(mat.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumMatrices;

    public MatrixListChunk(EndianAwareBinaryReader br) : this(ListHelper.ReadArray(br.ReadInt32(), () => new Matrix(br)))
    {
    }

    public MatrixListChunk(IList<Matrix> matrices) : base(ChunkID)
    {
        Matrices = CreateSizeAwareList(matrices);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumMatrices)
            yield return new InvalidP3DException(this, $"Num Matrices value {NumMatrices} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumMatrices);
        foreach (var mat in Matrices)
            mat.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var matrices = new Matrix[Matrices.Count];
        for (var i = 0; i < Matrices.Count; i++)
            matrices[i] = Matrices[i].Clone();
        return new MatrixListChunk(matrices);
    }

    public class Matrix
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }

        public byte[] DataBytes => [D, C, B, A];

        public Matrix(BinaryReader br)
        {
            D = br.ReadByte();
            C = br.ReadByte();
            B = br.ReadByte();
            A = br.ReadByte();
        }

        public Matrix(byte a, byte b, byte c, byte d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Matrix()
        {
            A = 0;
            B = 0;
            C = 0;
            D = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(D);
            bw.Write(C);
            bw.Write(B);
            bw.Write(A);
        }

        internal Matrix Clone() => new(A, B, C, D);

        public override string ToString() => $"{A} | {B} | {C} | {D}";
    }
}