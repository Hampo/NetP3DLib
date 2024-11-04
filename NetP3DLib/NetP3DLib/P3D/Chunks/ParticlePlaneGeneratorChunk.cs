using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ParticlePlaneGeneratorChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Particle_Plane_Generator;
    
    public uint Version { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float HorizontalSpread { get; set; }
    public float VerticalSpread { get; set; }
    public float RadialVar { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Width));
            data.AddRange(BitConverter.GetBytes(Height));
            data.AddRange(BitConverter.GetBytes(HorizontalSpread));
            data.AddRange(BitConverter.GetBytes(VerticalSpread));
            data.AddRange(BitConverter.GetBytes(RadialVar));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float);

    public ParticlePlaneGeneratorChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Width = br.ReadSingle();
        Height = br.ReadSingle();
        HorizontalSpread = br.ReadSingle();
        VerticalSpread = br.ReadSingle();
        RadialVar = br.ReadSingle();
    }

    public ParticlePlaneGeneratorChunk(uint version, float width, float height, float horizontalSpread, float verticalSpread, float radialVar) : base(ChunkID)
    {
        Version = version;
        Width = width;
        Height = height;
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
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(HorizontalSpread);
        bw.Write(VerticalSpread);
        bw.Write(RadialVar);
    }
}