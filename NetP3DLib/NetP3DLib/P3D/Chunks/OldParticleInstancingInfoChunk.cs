using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldParticleInstancingInfoChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Particle_Instancing_Info;
    
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

    public OldParticleInstancingInfoChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        MaxInstances = br.ReadUInt32();
    }

    public OldParticleInstancingInfoChunk(uint version, uint maxInstances) : base(ChunkID)
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