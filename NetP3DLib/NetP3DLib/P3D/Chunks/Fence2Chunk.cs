using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Fence_2)]
public class Fence2Chunk : Chunk
{
    public Vector3 Start { get; set; }
    public Vector3 End { get; set; }
    public Vector3 Normal { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Start));
            data.AddRange(BinaryExtensions.GetBytes(End));
            data.AddRange(BinaryExtensions.GetBytes(Normal));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3;

    public Fence2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Fence_2)
    {
        Start = br.ReadVector3();
        End = br.ReadVector3();
        Normal = br.ReadVector3();
    }

    public Fence2Chunk(Vector3 start, Vector3 end, Vector3 normal) : base((uint)ChunkIdentifier.Fence_2)
    {
        Start = start;
        End = end;
        Normal = normal;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Start);
        bw.Write(End);
        bw.Write(Normal);
    }
}