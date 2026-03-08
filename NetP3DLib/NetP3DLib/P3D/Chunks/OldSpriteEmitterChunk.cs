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
public class OldSpriteEmitterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Sprite_Emitter;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    private readonly P3DString _shaderName;
    public string ShaderName
    {
        get => _shaderName?.Value ?? string.Empty;
        set => _shaderName.Value = value;
    }
    private readonly FourCC _angleMode;
    [MaxLength(4)]
    public string AngleMode
    {
        get => _angleMode?.Value ?? string.Empty;
        set => _angleMode.Value = value;
    }
    public float Angle { get; set; }
    private readonly FourCC _textureAnimMode;
    [MaxLength(4)]
    public string TextureAnimMode
    {
        get => _textureAnimMode?.Value ?? string.Empty;
        set => _textureAnimMode.Value = value;
    }
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

    public OldSpriteEmitterChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        _shaderName = new(this, br);
        _angleMode = new(this, br);
        Angle = br.ReadSingle();
        _textureAnimMode = new(this, br);
        NumTextureFrames = br.ReadUInt32();
        TextureFrameRate = br.ReadUInt32();
    }

    public OldSpriteEmitterChunk(uint version, string name, string shaderName, string angleMode, float angle, string textureAnimMode, uint numTextureFrames, uint textureFrameRate) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _shaderName = new(this, shaderName);
        _angleMode = new(this, angleMode);
        Angle = angle;
        _textureAnimMode = new(this, textureAnimMode);
        NumTextureFrames = numTextureFrames;
        TextureFrameRate = textureFrameRate;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!ShaderName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ShaderName), ShaderName);

        if (!AngleMode.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(AngleMode), AngleMode);

        if (!TextureAnimMode.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(TextureAnimMode), TextureAnimMode);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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

    protected override Chunk CloneSelf() => new OldSpriteEmitterChunk(Version, Name, ShaderName, AngleMode, Angle, TextureAnimMode, NumTextureFrames, TextureFrameRate);
}
