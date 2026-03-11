using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BoundingBoxChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Bounding_Box;

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

    public BoundingBoxChunk(EndianAwareBinaryReader br) : this(br.ReadVector3(), br.ReadVector3())
    {
    }

    public BoundingBoxChunk(Vector3 low, Vector3 high) : base(ChunkID)
    {
        Low = low;
        High = high;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Low);
        bw.Write(High);
    }

    protected override Chunk CloneSelf() => new BoundingBoxChunk(Low, High);
}