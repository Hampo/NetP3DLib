using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureFontChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Texture_Font;
    
    public uint Version { get; set; }
    public string Shader { get; set; }
    public float FontSize { get; set; }
    public float FontWidth { get; set; }
    public float FontHeight { get; set; }
    public float FontBaseLine { get; set; }
    public uint NumTextures => GetChildCount(ChunkIdentifier.Texture);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Shader));
            data.AddRange(BitConverter.GetBytes(FontSize));
            data.AddRange(BitConverter.GetBytes(FontWidth));
            data.AddRange(BitConverter.GetBytes(FontHeight));
            data.AddRange(BitConverter.GetBytes(FontBaseLine));
            data.AddRange(BitConverter.GetBytes(NumTextures));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(Shader) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public TextureFontChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Shader = br.ReadP3DString();
        FontSize = br.ReadSingle();
        FontWidth = br.ReadSingle();
        FontHeight = br.ReadSingle();
        FontBaseLine = br.ReadSingle();
        var numTexture = br.ReadUInt32();
    }

    public TextureFontChunk(uint version, string name, string shader, float fontSize, float fontWidth, float fontHeight, float fontBaseLine) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Shader = shader;
        FontSize = fontSize;
        FontWidth = fontWidth;
        FontHeight = fontHeight;
        FontBaseLine = fontBaseLine;
    }

    public override void Validate()
    {
        if (!Shader.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Shader), Shader);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Shader);
        bw.Write(FontSize);
        bw.Write(FontWidth);
        bw.Write(FontHeight);
        bw.Write(FontBaseLine);
        bw.Write(NumTextures);
    }

    internal override Chunk CloneSelf() => new TextureFontChunk(Version, Name, Shader, FontSize, FontWidth, FontHeight, FontBaseLine);
}