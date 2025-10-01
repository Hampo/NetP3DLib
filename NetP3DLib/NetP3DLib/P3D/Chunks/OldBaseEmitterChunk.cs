using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBaseEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Base_Emitter;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    [MaxLength(4)]
    public string ParticleType { get; set; }
    [MaxLength(4)]
    public string GeneratorType { get; set; }
    private uint zTest;
    public bool ZTest
    {
        get => zTest == 1;
        set => zTest = value ? 1u : 0u;
    }
    private uint zWrite;
    public bool ZWrite
    {
        get => zWrite == 1;
        set => zWrite = value ? 1u : 0u;
    }
    private uint fog;
    public bool Fog
    {
        get => fog == 1;
        set => fog = value ? 1u : 0u;
    }
    public uint MaxParticles { get; set; }
    private uint infiniteLife;
    public bool InfiniteLife
    {
        get => infiniteLife == 1;
        set => infiniteLife = value ? 1u : 0u;
    }
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
            data.AddRange(BitConverter.GetBytes(zTest));
            data.AddRange(BitConverter.GetBytes(zWrite));
            data.AddRange(BitConverter.GetBytes(fog));
            data.AddRange(BitConverter.GetBytes(MaxParticles));
            data.AddRange(BitConverter.GetBytes(infiniteLife));
            data.AddRange(BitConverter.GetBytes(RotationalCohesion));
            data.AddRange(BitConverter.GetBytes(TranslationCohesion));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + 4 + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float);

    public OldBaseEmitterChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ParticleType = br.ReadFourCC();
        GeneratorType = br.ReadFourCC();
        zTest = br.ReadUInt32();
        zWrite = br.ReadUInt32();
        fog = br.ReadUInt32();
        MaxParticles = br.ReadUInt32();
        infiniteLife = br.ReadUInt32();
        RotationalCohesion = br.ReadSingle();
        TranslationCohesion = br.ReadSingle();
    }

    public OldBaseEmitterChunk(uint version, string name, string particleType, string generatorType, bool zTest, bool zWrite, bool fog, uint maxParticles, bool infiniteLife, float rotationalCohesion, float translationCohesion) : base(ChunkID)
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
        if (!ParticleType.IsValidFourCC())
            throw new InvalidFourCCException(nameof(ParticleType), ParticleType);

        if (!GeneratorType.IsValidFourCC())

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(ParticleType);
        bw.WriteFourCC(GeneratorType);
        bw.Write(zTest);
        bw.Write(zWrite);
        bw.Write(fog);
        bw.Write(MaxParticles);
        bw.Write(infiniteLife);
        bw.Write(RotationalCohesion);
        bw.Write(TranslationCohesion);
    }

    protected override Chunk CloneSelf() => new OldBaseEmitterChunk(Version, Name, ParticleType, GeneratorType, ZTest, ZWrite, Fog, MaxParticles, InfiniteLife, RotationalCohesion, TranslationCohesion);
}
