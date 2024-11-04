using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Animation_Sync_Frame)]
public class AnimationSyncFrameChunk : Chunk
{
    public uint Version { get; set; }
    public float SyncFrame { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(SyncFrame));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float);

    public AnimationSyncFrameChunk(BinaryReader br) : base((uint)ChunkIdentifier.Animation_Sync_Frame)
    {
        Version = br.ReadUInt32();
        SyncFrame = br.ReadSingle();
    }

    public AnimationSyncFrameChunk(uint version, float syncFrame) : base((uint)ChunkIdentifier.Animation_Sync_Frame)
    {
        Version = version;
        SyncFrame = syncFrame;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(SyncFrame);
    }
}