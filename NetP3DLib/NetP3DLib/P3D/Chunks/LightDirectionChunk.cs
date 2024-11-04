using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightDirectionChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Direction;
    
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

    public LightDirectionChunk(BinaryReader br) : base(ChunkID)
    {
        Direction = br.ReadVector3();
    }

    public LightDirectionChunk(Vector3 direction) : base(ChunkID)
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