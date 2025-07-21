using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BillboardTextureUVChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Billboard_Texture_UV;
    
    public uint Version { get; set; }
    public uint RandomU { get; set; }
    public uint RandomV { get; set; }
    public Vector2 UV0 { get; set; }
    public Vector2 UV1 { get; set; }
    public Vector2 UV2 { get; set; }
    public Vector2 UV3 { get; set; }
    public Vector2 UVOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(RandomU));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) * 2;

    public BillboardTextureUVChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        RandomU = br.ReadUInt32();
        RandomV = br.ReadUInt32();
        UV0 = br.ReadVector2();
        UV1 = br.ReadVector2();
        UV2 = br.ReadVector2();
        UV3 = br.ReadVector2();
        UVOffset = br.ReadVector2();
    }

    public BillboardTextureUVChunk(uint version, uint randomU, uint randomV, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uvOffset) : base(ChunkID)
    {
        Version = version;
        RandomU = randomU;
        RandomV = randomV;
        UV0 = uv0;
        UV1 = uv1;
        UV2 = uv2;
        UV3 = uv3;
        UVOffset = uvOffset;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(RandomU);
        bw.Write(RandomV);
        bw.Write(UV0);
        bw.Write(UV1);
        bw.Write(UV2);
        bw.Write(UV3);
        bw.Write(UVOffset);
    }
}