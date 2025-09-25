using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightShadowChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Shadow;

    private uint shadow;
    public bool Shadow
    {
        get => shadow == 1;
        set => shadow = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(shadow));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public LightShadowChunk(BinaryReader br) : base(ChunkID)
    {
        shadow = br.ReadUInt32();
    }

    public LightShadowChunk(bool shadow) : base(ChunkID)
    {
        Shadow = shadow;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(shadow);
    }

    internal override Chunk CloneSelf() => new LightShadowChunk(Shadow);
}