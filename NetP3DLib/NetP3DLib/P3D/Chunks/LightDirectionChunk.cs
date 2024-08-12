using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Light_Direction)]
public class LightDirectionChunk : Chunk
{
    Vector3 Direction { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Direction));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public LightDirectionChunk(BinaryReader br) : base((uint)ChunkIdentifier.Light_Direction)
    {
        Direction = br.ReadVector3();
    }

    public LightDirectionChunk(Vector3 direction) : base((uint)ChunkIdentifier.Light_Direction)
    {
        Direction = direction;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Direction);
    }
}