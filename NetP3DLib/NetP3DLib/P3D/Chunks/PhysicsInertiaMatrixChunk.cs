using NetP3DLib.P3D.Attributes;
using NetP3DLib.Numerics;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhysicsInertiaMatrixChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Physics_Inertia_Matrix;
    
    public SymmetricMatrix3x3 Matrix { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Matrix));

            return [.. data];
        }
    }
    public override uint DataLength => SymmetricMatrix3x3.SizeInBytes;

    public PhysicsInertiaMatrixChunk(BinaryReader br) : base(ChunkID)
    {
        Matrix = br.ReadSymmetricMatrix3x3();
    }

    public PhysicsInertiaMatrixChunk(SymmetricMatrix3x3 matrix) : base(ChunkID)
    {
        Matrix = matrix;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Matrix);
    }

    internal override Chunk CloneSelf() => new PhysicsInertiaMatrixChunk(Matrix);
}