using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Particle_Animation)]
public class OldParticleAnimationChunk : Chunk
{
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

    public OldParticleAnimationChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Particle_Animation)
    {
        Version = br.ReadUInt32();
    }

    public OldParticleAnimationChunk(uint version) : base((uint)ChunkIdentifier.Old_Particle_Animation)
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