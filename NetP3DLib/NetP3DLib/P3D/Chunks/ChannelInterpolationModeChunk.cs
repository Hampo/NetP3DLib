using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ChannelInterpolationModeChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Channel_Interpolation_Mode;
    
    public uint Version { get; set; }
    public uint Interpolate { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Interpolate));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public ChannelInterpolationModeChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Interpolate = br.ReadUInt32();
    }

    public ChannelInterpolationModeChunk(uint version, uint interpolate) : base(ChunkID)
    {
        Version = version;
        Interpolate = interpolate;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Interpolate);
    }
}