using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Particle_Instancing_Info)]
public class OldParticleInstancingInfoChunk : Chunk
{
    public uint Version { get; set; }
    public uint MaxInstances { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(MaxInstances));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public OldParticleInstancingInfoChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Particle_Instancing_Info)
    {
        Version = br.ReadUInt32();
        MaxInstances = br.ReadUInt32();
    }

    public OldParticleInstancingInfoChunk(uint version, uint maxInstances) : base((uint)ChunkIdentifier.Old_Particle_Instancing_Info)
    {
        Version = version;
        MaxInstances = maxInstances;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(MaxInstances);
    }
}