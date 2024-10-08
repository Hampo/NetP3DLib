using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Locator_Matrix)]
public class LocatorMatrixChunk : Chunk
{
    public Matrix4x4 Matrix { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Matrix));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 16;

    public LocatorMatrixChunk(BinaryReader br) : base((uint)ChunkIdentifier.Locator_Matrix)
    {
        Matrix = br.ReadMatrix4x4();
    }

    public LocatorMatrixChunk(Matrix4x4 matrix) : base((uint)ChunkIdentifier.Locator_Matrix)
    {
        Matrix = matrix;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Matrix);
    }
}