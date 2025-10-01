using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldParticleAnimationChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Particle_Animation;
    
    [DefaultValue(0)]
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

    public OldParticleAnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
    }

    public OldParticleAnimationChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
    }

    protected override Chunk CloneSelf() => new OldParticleAnimationChunk(Version);
}
