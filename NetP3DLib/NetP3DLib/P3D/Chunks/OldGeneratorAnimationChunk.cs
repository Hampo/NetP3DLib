using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldGeneratorAnimationChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Old_Generator_Animation;
    
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

    public OldGeneratorAnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
    }

    public OldGeneratorAnimationChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
    }
}