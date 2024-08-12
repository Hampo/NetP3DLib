using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Render_Status)]
public class RenderStatusChunk : Chunk
{
    public uint CastShadow { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(CastShadow));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public RenderStatusChunk(BinaryReader br) : base((uint)ChunkIdentifier.Render_Status)
    {
        CastShadow = br.ReadUInt32();
    }

    public RenderStatusChunk(uint castShadow) : base((uint)ChunkIdentifier.Render_Status)
    {
        CastShadow = castShadow;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(CastShadow);
    }
}