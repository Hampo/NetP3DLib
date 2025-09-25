using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ChannelInterpolationModeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Channel_Interpolation_Mode;
    
    public uint Version { get; set; }
    private uint interpolate;
    public bool Interpolate
    {
        get => interpolate != 0;
        set => interpolate = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(interpolate));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public ChannelInterpolationModeChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        interpolate = br.ReadUInt32();
    }

    public ChannelInterpolationModeChunk(uint version, bool interpolate) : base(ChunkID)
    {
        Version = version;
        Interpolate = interpolate;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(interpolate);
    }

    internal override Chunk CloneSelf() => new ChannelInterpolationModeChunk(Version, Interpolate);
}