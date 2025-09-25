using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldSpriteEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Sprite_Emitter;
    
    public uint Version { get; set; }
    public string ShaderName { get; set; }
    public string AngleMode { get; set; }
    public float Angle { get; set; }
    public string TextureAnimMode { get; set; }
    public uint NumTextureFrames { get; set; }
    public uint TextureFrameRate { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ShaderName));
            data.AddRange(BinaryExtensions.GetFourCCBytes(AngleMode));
            data.AddRange(BitConverter.GetBytes(Angle));
            data.AddRange(BinaryExtensions.GetFourCCBytes(TextureAnimMode));
            data.AddRange(BitConverter.GetBytes(NumTextureFrames));
            data.AddRange(BitConverter.GetBytes(TextureFrameRate));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(ShaderName) + 4 + sizeof(float) + 4 + sizeof(uint) + sizeof(uint);

    public OldSpriteEmitterChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ShaderName = br.ReadP3DString();
        AngleMode = br.ReadFourCC();
        Angle = br.ReadSingle();
        TextureAnimMode = br.ReadFourCC();
        NumTextureFrames = br.ReadUInt32();
        TextureFrameRate = br.ReadUInt32();
    }

    public OldSpriteEmitterChunk(uint version, string name, string shaderName, string angleMode, float angle, string textureAnimMode, uint numTextureFrames, uint textureFrameRate) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ShaderName = shaderName;
        AngleMode = angleMode;
        Angle = angle;
        TextureAnimMode = textureAnimMode;
        NumTextureFrames = numTextureFrames;
        TextureFrameRate = textureFrameRate;
    }

    public override void Validate()
    {
        if (!ShaderName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ShaderName), ShaderName);

        if (!AngleMode.IsValidFourCC())
            throw new InvalidFourCCException(nameof(AngleMode), AngleMode);

        if (!TextureAnimMode.IsValidFourCC())
            throw new InvalidFourCCException(nameof(TextureAnimMode), TextureAnimMode);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ShaderName);
        bw.WriteFourCC(AngleMode);
        bw.Write(Angle);
        bw.WriteFourCC(TextureAnimMode);
        bw.Write(NumTextureFrames);
        bw.Write(TextureFrameRate);
    }

    internal override Chunk CloneSelf() => new OldSpriteEmitterChunk(Version, Name, ShaderName, AngleMode, Angle, TextureAnimMode, NumTextureFrames, TextureFrameRate);
}