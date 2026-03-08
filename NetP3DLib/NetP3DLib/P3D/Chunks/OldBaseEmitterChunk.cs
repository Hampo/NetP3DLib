using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
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
    private readonly FourCC _particleType;
    [MaxLength(4)]
    public string ParticleType
    {
        get => _particleType?.Value ?? string.Empty;
        set => _particleType.Value = value;
    }
    private readonly FourCC _generatorType;
    [MaxLength(4)]
    public string GeneratorType
    {
        get => _generatorType?.Value ?? string.Empty;
        set => _generatorType.Value = value;
    }
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
        _name = new(this, br);
        _particleType = new(this, br);
        _generatorType = new(this, br);
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
        _name = new(this, name);
        _particleType = new(this, particleType);
        _generatorType = new(this, generatorType);
        ZTest = zTest;
        ZWrite = zWrite;
        Fog = fog;
        MaxParticles = maxParticles;
        InfiniteLife = infiniteLife;
        RotationalCohesion = rotationalCohesion;
        TranslationCohesion = translationCohesion;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!ParticleType.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(ParticleType), ParticleType);

        if (!GeneratorType.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this, nameof(GeneratorType), GeneratorType);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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
