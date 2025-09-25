using NetP3DLib.P3D.Attributes;
using NetP3DLib.IO;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Image;
    
    public enum Formats
    {
        Raw = 0,
        PNG = 1,
        TGA = 2,
        BMP = 3,
        IPU = 4,
        DXT = 5,
        DXT1 = 6,
        DXT2 = 7,
        DXT3 = 8,
        DXT4 = 9,
        DXT5 = 10,
        PS24Bit = 11,
        PS28Bit = 12,
        PS216Bit = 13,
        PS232Bit = 14,
        GC4Bit = 15,
        GC8Bit = 16,
        GC16Bit = 17,
        GC32Bit = 18,
        GCDXT1 = 19,
        Other = 20,
        Invalid = 21,
        PSP4Bit = 25,
    }

    [DefaultValue(14000)]
    public uint Version { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint Bpp { get; set; }
    private uint palettized;
    public bool Palettized
    {
        get => palettized == 1;
        set => palettized = value ? 1u : 0u;
    }
    private uint hasAlpha;
    public bool HasAlpha
    {
        get => hasAlpha == 1;
        set => hasAlpha = value ? 1u : 0u;
    }
    public Formats Format { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Width));
            data.AddRange(BitConverter.GetBytes(Height));
            data.AddRange(BitConverter.GetBytes(Bpp));
            data.AddRange(BitConverter.GetBytes(palettized));
            data.AddRange(BitConverter.GetBytes(hasAlpha));
            data.AddRange(BitConverter.GetBytes((uint)Format));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public ImageChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Width = br.ReadUInt32();
        Height = br.ReadUInt32();
        Bpp = br.ReadUInt32();
        palettized = br.ReadUInt32();
        hasAlpha = br.ReadUInt32();
        Format = (Formats)br.ReadUInt32();
    }

    public ImageChunk(string name, uint version, uint width, uint height, uint bpp, bool palettized, bool hasAlpha, Formats format) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Width = width;
        Height = height;
        Bpp = bpp;
        Palettized = palettized;
        HasAlpha = hasAlpha;
        Format = format;
    }

    private static readonly HashSet<Formats> FlippedFormats = [
        Formats.PS216Bit,
        Formats.PS232Bit,
        Formats.GC16Bit,
        Formats.GC32Bit,
        Formats.Raw,
        Formats.DXT1,
        Formats.GCDXT1,
        Formats.DXT3,
        Formats.DXT5,
    ];
    internal override void WriteData(BinaryWriter bw)
    {
        if (bw is EndianAwareBinaryWriter endianAwareBinaryWriter && endianAwareBinaryWriter.Endianness != BinaryExtensions.DefaultEndian && FlippedFormats.Contains(Format))
            throw new NotImplementedException($"Writing {Format} images with swapped endian is not supported at this time.");

        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(Bpp);
        bw.Write(palettized);
        bw.Write(hasAlpha);
        bw.Write((uint)Format);
    }

    internal override Chunk CloneSelf() => new ImageChunk(Name, Version, Width, Height, Bpp, Palettized, HasAlpha, Format);
}
