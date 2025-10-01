using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SpriteParticleEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Sprite_Particle_Emitter;
    
    public uint Version { get; set; }
    public string ShaderName { get; set; }
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

    public SpriteParticleEmitterChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ShaderName = br.ReadP3DString();
        UpdateMode = br.ReadUInt32();
        ZTest = br.ReadUInt32();
        ZWrite = br.ReadUInt32();
        MaxParticles = br.ReadUInt32();
        InfiniteLife = br.ReadUInt32();
        NumTextureFrames = br.ReadUInt32();
        TextureFrameRate = br.ReadUInt32();
        InitialAngle = br.ReadSingle();
        InitialAngleVariance = br.ReadSingle();
        TranslationalCohesion = br.ReadSingle();
        RotationalCohesion = br.ReadSingle();
    }

    public SpriteParticleEmitterChunk(uint version, string name, string shaderName, uint updateMode, uint zTest, uint zWrite, uint maxParticles, uint infiniteLife, uint numTextureFrames, uint textureFrameRate, float initialAngle, float initialAngleVariance, float translationalCohesion, float rotationalCohesion) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ShaderName = shaderName;
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

    public override void Validate()
    {
        if (!ShaderName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ShaderName), ShaderName);

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
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