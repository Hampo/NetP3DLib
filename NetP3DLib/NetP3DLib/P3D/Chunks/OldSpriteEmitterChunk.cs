using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Sprite_Emitter)]
public class OldSpriteEmitterChunk : NamedChunk
{
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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(ShaderName).Length + 4 + sizeof(float) + 4 + sizeof(uint) + sizeof(uint);

    public OldSpriteEmitterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Sprite_Emitter)
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

    public OldSpriteEmitterChunk(uint version, string name, string shaderName, string angleMode, float angle, string textureAnimMode, uint numTextureFrames, uint textureFrameRate) : base((uint)ChunkIdentifier.Old_Sprite_Emitter)
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
        if (ShaderName == null)
            throw new InvalidDataException($"{nameof(ShaderName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ShaderName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ShaderName)} is 255 bytes.");

        if (AngleMode == null || AngleMode.Length == 0)
            throw new InvalidDataException($"{nameof(AngleMode)} must be at least 1 char.");
        if (AngleMode.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(AngleMode)} is 4 chars.");

        if (TextureAnimMode == null || TextureAnimMode.Length == 0)
            throw new InvalidDataException($"{nameof(TextureAnimMode)} must be at least 1 char.");
        if (TextureAnimMode.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(TextureAnimMode)} is 4 chars.");

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
}