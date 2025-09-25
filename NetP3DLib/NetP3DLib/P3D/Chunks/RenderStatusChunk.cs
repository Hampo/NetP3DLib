using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RenderStatusChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Render_Status;

    private uint castShadow;
    public bool CastShadow
    {
        get => castShadow == 0;
        set => castShadow = value ? 0u : 1u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(castShadow));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public RenderStatusChunk(BinaryReader br) : base(ChunkID)
    {
        castShadow = br.ReadUInt32();
    }

    public RenderStatusChunk(bool castShadow) : base(ChunkID)
    {
        CastShadow = castShadow;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(castShadow);
    }

    internal override Chunk CloneSelf() => new RenderStatusChunk(CastShadow);
}