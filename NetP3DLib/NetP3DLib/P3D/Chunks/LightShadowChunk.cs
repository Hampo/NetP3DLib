using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Light_Shadow)]
public class LightShadowChunk : Chunk
{
    public uint Shadow { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Shadow));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public LightShadowChunk(BinaryReader br) : base((uint)ChunkIdentifier.Light_Shadow)
    {
        Shadow = br.ReadUInt32();
    }

    public LightShadowChunk(uint shadow) : base((uint)ChunkIdentifier.Light_Shadow)
    {
        Shadow = shadow;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Shadow);
    }
}