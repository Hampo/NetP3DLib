using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Texture_Font)]
public class TextureFontChunk : NamedChunk
{
    public uint Version { get; set; }
    public string Shader { get; set; }
    public float FontSize { get; set; }
    public float FontWidth { get; set; }
    public float FontHeight { get; set; }
    public float FontBaseLine { get; set; }
    public uint NumTextures => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Texture).Count();

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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(Shader).Length + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public TextureFontChunk(BinaryReader br) : base((uint)ChunkIdentifier.Texture_Font)
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

    public TextureFontChunk(uint version, string name, string shader, float fontSize, float fontWidth, float fontHeight, float fontBaseLine) : base((uint)ChunkIdentifier.Texture_Font)
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
        if (Shader == null)
            throw new InvalidDataException($"{nameof(Shader)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Shader).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Shader)} is 255 bytes.");

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
}