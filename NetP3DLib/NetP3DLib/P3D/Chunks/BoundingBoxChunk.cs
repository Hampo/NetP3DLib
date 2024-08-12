using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Bounding_Box)]
public class BoundingBoxChunk : Chunk
{
    public Vector3 Low { get; set; }
    public Vector3 High { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Low));
            data.AddRange(BinaryExtensions.GetBytes(High));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3 + sizeof(float) * 3;

    public BoundingBoxChunk(BinaryReader br) : base((uint)ChunkIdentifier.Bounding_Box)
    {
        Low = br.ReadVector3();
        High = br.ReadVector3();
    }

    public BoundingBoxChunk(Vector3 low, Vector3 high) : base((uint)ChunkIdentifier.Bounding_Box)
    {
        Low = low;
        High = high;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Low);
        bw.Write(High);
    }
}