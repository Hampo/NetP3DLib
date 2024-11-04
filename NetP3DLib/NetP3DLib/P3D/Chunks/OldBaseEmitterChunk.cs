using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBaseEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Base_Emitter;
    
    public uint Version { get; set; }
    public string ParticleType { get; set; }
    public string GeneratorType { get; set; }
    public uint ZTest { get; set; }
    public uint ZWrite { get; set; }
    public uint Fog { get; set; }
    public uint MaxParticles { get; set; }
    public uint InfiniteLife { get; set; }
    public float RotationalCohesion { get; set; }
    public float TranslationCohesion { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(ParticleType));
            data.AddRange(BinaryExtensions.GetFourCCBytes(GeneratorType));
            data.AddRange(BitConverter.GetBytes(ZTest));
            data.AddRange(BitConverter.GetBytes(ZWrite));
            data.AddRange(BitConverter.GetBytes(Fog));
            data.AddRange(BitConverter.GetBytes(MaxParticles));
            data.AddRange(BitConverter.GetBytes(InfiniteLife));
            data.AddRange(BitConverter.GetBytes(RotationalCohesion));
            data.AddRange(BitConverter.GetBytes(TranslationCohesion));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + 4 + 4 + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float);

    public OldBaseEmitterChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ParticleType = br.ReadFourCC();
        GeneratorType = br.ReadFourCC();
        ZTest = br.ReadUInt32();
        ZWrite = br.ReadUInt32();
        Fog = br.ReadUInt32();
        MaxParticles = br.ReadUInt32();
        InfiniteLife = br.ReadUInt32();
        RotationalCohesion = br.ReadSingle();
        TranslationCohesion = br.ReadSingle();
    }

    public OldBaseEmitterChunk(uint version, string name, string particleType, string generatorType, uint zTest, uint zWrite, uint fog, uint maxParticles, uint infiniteLife, float rotationalCohesion, float translationCohesion) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ParticleType = particleType;
        GeneratorType = generatorType;
        ZTest = zTest;
        ZWrite = zWrite;
        Fog = fog;
        MaxParticles = maxParticles;
        InfiniteLife = infiniteLife;
        RotationalCohesion = rotationalCohesion;
        TranslationCohesion = translationCohesion;
    }

    public override void Validate()
    {
        if (ParticleType == null || ParticleType.Length == 0)
            throw new InvalidDataException($"{nameof(ParticleType)} must be at least 1 char.");

        if (ParticleType.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(ParticleType)} is 4 chars.");

        if (GeneratorType == null || GeneratorType.Length == 0)
            throw new InvalidDataException($"{nameof(GeneratorType)} must be at least 1 char.");

        if (GeneratorType.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(GeneratorType)} is 4 chars.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(ParticleType);
        bw.WriteFourCC(GeneratorType);
        bw.Write(ZTest);
        bw.Write(ZWrite);
        bw.Write(Fog);
        bw.Write(MaxParticles);
        bw.Write(InfiniteLife);
        bw.Write(RotationalCohesion);
        bw.Write(TranslationCohesion);
    }
}