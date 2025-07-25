using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Texture;

    public enum TextureTypes : uint
    {
        RGB,
        Palettized,
        Luminance,
        BumpMap,
        DXT1,
        DXT2,
        DXT3,
        DXT4,
        DXT5,
        IPU,
        Z,
        Linear,
        RenderTarget,
        PS2_4Bit,
        PS2_8Bit,
        PS2_16Bit,
        PS2_32Bit,
        GC_4Bit,
        GC_8Bit,
        GC_16Bit,
        GC_32Bit,
        GC_DXT1,
    }

    public enum UsageHints : uint
    {
        Static,
        Dyanmic,
        NoCache,
    }
    
    public uint Version { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint Bpp { get; set; }
    public uint AlphaDepth { get; set; }
    public uint NumMipMaps { get; set; }
    public TextureTypes TextureType { get; set; }
    public UsageHints UsageHint { get; set; }
    public uint Priority { get; set; }

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
            data.AddRange(BitConverter.GetBytes(AlphaDepth));
            data.AddRange(BitConverter.GetBytes(NumMipMaps));
            data.AddRange(BitConverter.GetBytes((uint)TextureType));
            data.AddRange(BitConverter.GetBytes((uint)UsageHint));
            data.AddRange(BitConverter.GetBytes(Priority));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public TextureChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Width = br.ReadUInt32();
        Height = br.ReadUInt32();
        Bpp = br.ReadUInt32();
        AlphaDepth = br.ReadUInt32();
        NumMipMaps = br.ReadUInt32();
        TextureType = (TextureTypes)br.ReadUInt32();
        UsageHint = (UsageHints)br.ReadUInt32();
        Priority = br.ReadUInt32();
    }
    public TextureChunk(string name, uint version, uint width, uint height, uint bpp, uint alphaDepth, uint numMipMaps, TextureTypes textureType, UsageHints usageHint, uint priority) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Width = width;
        Height = height;
        Bpp = bpp;
        AlphaDepth = alphaDepth;
        NumMipMaps = numMipMaps;
        TextureType = textureType;
        UsageHint = usageHint;
        Priority = priority;
    }

    public override void Validate()
    {
        if (!Width.IsPowerOfTwo())
            throw new InvalidDataException($"{nameof(Width)} must be a power of 2.");

        if (!Height.IsPowerOfTwo())
            throw new InvalidDataException($"{nameof(Height)} must be a power of 2.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(Bpp);
        bw.Write(AlphaDepth);
        bw.Write(NumMipMaps);
        bw.Write((uint)TextureType);
        bw.Write((uint)UsageHint);
        bw.Write(Priority);
    }

    internal override Chunk CloneSelf() => new TextureChunk(Name, Version, Width, Height, Bpp, AlphaDepth, NumMipMaps, TextureType, UsageHint, Priority);
}