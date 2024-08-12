using NetP3DLib.Numerics;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Physics_Inertia_Matrix)]
public class PhysicsInertiaMatrixChunk : Chunk
{
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

    public PhysicsInertiaMatrixChunk(BinaryReader br) : base((uint)ChunkIdentifier.Physics_Inertia_Matrix)
    {
        Matrix = br.ReadSymmetricMatrix3x3();
    }

    public PhysicsInertiaMatrixChunk(SymmetricMatrix3x3 matrix) : base((uint)ChunkIdentifier.Physics_Inertia_Matrix)
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