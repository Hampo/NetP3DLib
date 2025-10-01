using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LocatorMatrixChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Locator_Matrix;
    
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

    public LocatorMatrixChunk(BinaryReader br) : base(ChunkID)
    {
        Matrix = br.ReadMatrix4x4();
    }

    public LocatorMatrixChunk(Matrix4x4 matrix) : base(ChunkID)
    {
        Matrix = matrix;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Matrix);
    }

    protected override Chunk CloneSelf() => new LocatorMatrixChunk(Matrix);
}