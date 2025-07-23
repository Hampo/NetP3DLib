using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageFontChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Image_Font;
    
    public uint Version { get; set; }
    public float FontSize { get; set; }
    public float FontWidth { get; set; }
    public float FontHeight { get; set; }
    public float FontBaseLine { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(FontSize));
            data.AddRange(BitConverter.GetBytes(FontWidth));
            data.AddRange(BitConverter.GetBytes(FontHeight));
            data.AddRange(BitConverter.GetBytes(FontBaseLine));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float);

    public ImageFontChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FontSize = br.ReadSingle();
        FontWidth = br.ReadSingle();
        FontHeight = br.ReadSingle();
        FontBaseLine = br.ReadSingle();
    }

    public ImageFontChunk(uint version, string name, float fontSize, float fontWidth, float fontHeight, float fontBaseLine) : base(ChunkID)
    {
        Version = version;
        Name = name;
        FontSize = fontSize;
        FontWidth = fontWidth;
        FontHeight = fontHeight;
        FontBaseLine = fontBaseLine;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(FontSize);
        bw.Write(FontWidth);
        bw.Write(FontHeight);
        bw.Write(FontBaseLine);
    }

    internal override Chunk CloneSelf() => new ImageFontChunk(Version, Name, FontSize, FontWidth, FontHeight, FontBaseLine);
}