using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SpriteParticleEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Sprite_Particle_Emitter;

    public uint Version { get; set; }
    private readonly P3DString _shaderName;
    public string ShaderName
    {
        get => _shaderName?.Value ?? string.Empty;
        set => _shaderName.Value = value;
    }
    public uint UpdateMode { get; set; }
    public uint ZTest { get; set; }
    public uint ZWrite { get; set; }
    public uint MaxParticles { get; set; }
    public uint InfiniteLife { get; set; }
    public uint NumTextureFrames { get; set; }
    public uint TextureFrameRate { get; set; }
    public float InitialAngle { get; set; }
    public float InitialAngleVariance { get; set; }
    public float TranslationalCohesion { get; set; }
    public float RotationalCohesion { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ShaderName));
            data.AddRange(BitConverter.GetBytes(UpdateMode));
            data.AddRange(BitConverter.GetBytes(ZTest));
            data.AddRange(BitConverter.GetBytes(ZWrite));
            data.AddRange(BitConverter.GetBytes(MaxParticles));
            data.AddRange(BitConverter.GetBytes(InfiniteLife));
            data.AddRange(BitConverter.GetBytes(NumTextureFrames));
            data.AddRange(BitConverter.GetBytes(TextureFrameRate));
            data.AddRange(BitConverter.GetBytes(InitialAngle));
            data.AddRange(BitConverter.GetBytes(InitialAngleVariance));
            data.AddRange(BitConverter.GetBytes(TranslationalCohesion));
            data.AddRange(BitConverter.GetBytes(RotationalCohesion));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(ShaderName) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float);

    public SpriteParticleEmitterChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadP3DString(), br.ReadP3DString(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
    {
    }

    public SpriteParticleEmitterChunk(uint version, string name, string shaderName, uint updateMode, uint zTest, uint zWrite, uint maxParticles, uint infiniteLife, uint numTextureFrames, uint textureFrameRate, float initialAngle, float initialAngleVariance, float translationalCohesion, float rotationalCohesion) : base(ChunkID, name)
    {
        Version = version;
        _shaderName = new(this, shaderName, nameof(ShaderName));
        UpdateMode = updateMode;
        ZTest = zTest;
        ZWrite = zWrite;
        MaxParticles = maxParticles;
        InfiniteLife = infiniteLife;
        NumTextureFrames = numTextureFrames;
        TextureFrameRate = textureFrameRate;
        InitialAngle = initialAngle;
        InitialAngleVariance = initialAngleVariance;
        TranslationalCohesion = translationalCohesion;
        RotationalCohesion = rotationalCohesion;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!ShaderName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ShaderName), ShaderName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ShaderName);
        bw.Write(UpdateMode);
        bw.Write(ZTest);
        bw.Write(ZWrite);
        bw.Write(MaxParticles);
        bw.Write(InfiniteLife);
        bw.Write(NumTextureFrames);
        bw.Write(TextureFrameRate);
        bw.Write(InitialAngle);
        bw.Write(InitialAngleVariance);
        bw.Write(TranslationalCohesion);
        bw.Write(RotationalCohesion);
    }

    protected override Chunk CloneSelf() => new SpriteParticleEmitterChunk(Version, Name, ShaderName, UpdateMode, ZTest, ZWrite, MaxParticles, InfiniteLife, NumTextureFrames, TextureFrameRate, InitialAngle, InitialAngleVariance, TranslationalCohesion, RotationalCohesion);
}