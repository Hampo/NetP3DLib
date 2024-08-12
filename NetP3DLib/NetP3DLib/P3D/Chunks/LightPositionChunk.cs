using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Light_Position)]
public class LightPositionChunk : Chunk
{
    public Vector3 Position;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetBytes(Position));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public LightPositionChunk(BinaryReader br) : base((uint)ChunkIdentifier.Light_Position)
    {
        Position = br.ReadVector3();
    }

    public LightPositionChunk(Vector3 position) : base((uint)ChunkIdentifier.Light_Position)
    {
        Position = position;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Position);
    }
}