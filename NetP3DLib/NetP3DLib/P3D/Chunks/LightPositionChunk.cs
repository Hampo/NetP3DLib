using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightPositionChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Position;
    
    public Vector3 Position { get; set; }

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

    public LightPositionChunk(BinaryReader br) : base(ChunkID)
    {
        Position = br.ReadVector3();
    }

    public LightPositionChunk(Vector3 position) : base(ChunkID)
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