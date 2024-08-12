using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Frontend_Project)]
public class FrontendProjectChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint ResolutionX { get; set; }
    public uint ResolutionY { get; set; }
    public string Platform { get; set; }
    public string PagePath { get; set; }
    public string ResourcePath { get; set; }
    public string ScreenPath { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(ResolutionX));
            data.AddRange(BitConverter.GetBytes(ResolutionY));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Platform));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(PagePath));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ResourcePath));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ScreenPath));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Platform).Length + (uint)BinaryExtensions.GetP3DStringBytes(PagePath).Length + (uint)BinaryExtensions.GetP3DStringBytes(ResourcePath).Length + (uint)BinaryExtensions.GetP3DStringBytes(ScreenPath).Length;

    public FrontendProjectChunk(BinaryReader br) : base((uint)ChunkIdentifier.Frontend_Project)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        ResolutionX = br.ReadUInt32();
        ResolutionY = br.ReadUInt32();
        Platform = br.ReadP3DString();
        PagePath = br.ReadP3DString();
        ResourcePath = br.ReadP3DString();
        ScreenPath = br.ReadP3DString();
    }

    public FrontendProjectChunk(string name, uint version, uint resolutionX, uint resolutionY, string platform, string pagePath, string resourcePath, string screenPath) : base((uint)ChunkIdentifier.Frontend_Project)
    {
        Name = name;
        Version = version;
        ResolutionX = resolutionX;
        ResolutionY = resolutionY;
        Platform = platform;
        PagePath = pagePath;
        ResourcePath = resourcePath;
        ScreenPath = screenPath;
    }

    public override void Validate()
    {
        if (Platform == null)
            throw new InvalidDataException($"{nameof(Platform)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Platform).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Platform)} is 255 bytes.");

        if (PagePath == null)
            throw new InvalidDataException($"{nameof(PagePath)} cannot be null.");
        if (Encoding.UTF8.GetBytes(PagePath).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(PagePath)} is 255 bytes.");

        if (ResourcePath == null)
            throw new InvalidDataException($"{nameof(ResourcePath)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ResourcePath).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ResourcePath)} is 255 bytes.");

        if (ScreenPath == null)
            throw new InvalidDataException($"{nameof(ScreenPath)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ScreenPath).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ScreenPath)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(ResolutionX);
        bw.Write(ResolutionY);
        bw.WriteP3DString(Platform);
        bw.WriteP3DString(PagePath);
        bw.WriteP3DString(ResourcePath);
        bw.WriteP3DString(ScreenPath);
    }
}