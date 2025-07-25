using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ParticlePointGeneratorChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Particle_Point_Generator;
    
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

    public ParticlePointGeneratorChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        HorizontalSpread = br.ReadSingle();
        VerticalSpread = br.ReadSingle();
        RadialVar = br.ReadSingle();
    }

    public ParticlePointGeneratorChunk(uint version, float horizontalSpread, float verticalSpread, float radialVar) : base(ChunkID)
    {
        Version = version;
        HorizontalSpread = horizontalSpread;
        VerticalSpread = verticalSpread;
        RadialVar = radialVar;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(HorizontalSpread);
        bw.Write(VerticalSpread);
        bw.Write(RadialVar);
    }

    internal override Chunk CloneSelf() => new ParticlePointGeneratorChunk(Version, HorizontalSpread, VerticalSpread, RadialVar);
}