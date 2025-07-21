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
    public int Interpolate { get; set; }

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
        Interpolate = br.ReadInt32();
    }

    public ChannelInterpolationModeChunk(uint version, int interpolate) : base(ChunkID)
    {
        Version = version;
        Interpolate = interpolate;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Interpolate);
    }
}