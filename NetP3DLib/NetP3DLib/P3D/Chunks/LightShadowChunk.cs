using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightShadowChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Shadow;
    
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

    public LightShadowChunk(BinaryReader br) : base(ChunkID)
    {
        Shadow = br.ReadUInt32();
    }

    public LightShadowChunk(uint shadow) : base(ChunkID)
    {
        Shadow = shadow;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Shadow);
    }
}