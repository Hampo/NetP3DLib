using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldEmitterAnimationChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Emitter_Animation;
    
    public uint Version { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public OldEmitterAnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
    }

    public OldEmitterAnimationChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
    }
}