using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class WallChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Wall;
    
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

    public WallChunk(BinaryReader br) : base(ChunkID)
    {
        Start = br.ReadVector3();
        End = br.ReadVector3();
        Normal = br.ReadVector3();
    }

    public WallChunk(Vector3 start, Vector3 end, Vector3 normal) : base(ChunkID)
    {
        Start = start;
        End = end;
        Normal = normal;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Start);
        bw.Write(End);
        bw.Write(Normal);
    }
}