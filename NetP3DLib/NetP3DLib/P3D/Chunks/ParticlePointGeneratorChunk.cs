using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Particle_Point_Generator)]
public class ParticlePointGeneratorChunk : Chunk
{
    public uint Version { get; set; }
    public float HorizontalSpread { get; set; }
    public float VerticalSpread { get; set; }
    public float RadialVar { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(HorizontalSpread));
            data.AddRange(BitConverter.GetBytes(VerticalSpread));
            data.AddRange(BitConverter.GetBytes(RadialVar));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float);

    public ParticlePointGeneratorChunk(BinaryReader br) : base((uint)ChunkIdentifier.Particle_Point_Generator)
    {
        Version = br.ReadUInt32();
        HorizontalSpread = br.ReadSingle();
        VerticalSpread = br.ReadSingle();
        RadialVar = br.ReadSingle();
    }

    public ParticlePointGeneratorChunk(uint version, float horizontalSpread, float verticalSpread, float radialVar) : base((uint)ChunkIdentifier.Particle_Point_Generator)
    {
        Version = version;
        HorizontalSpread = horizontalSpread;
        VerticalSpread = verticalSpread;
        RadialVar = radialVar;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(HorizontalSpread);
        bw.Write(VerticalSpread);
        bw.Write(RadialVar);
    }
}