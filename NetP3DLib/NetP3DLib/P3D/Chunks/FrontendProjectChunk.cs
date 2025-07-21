using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendProjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Project;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Platform) + BinaryExtensions.GetP3DStringLength(PagePath) + BinaryExtensions.GetP3DStringLength(ResourcePath) + BinaryExtensions.GetP3DStringLength(ScreenPath);

    public FrontendProjectChunk(BinaryReader br) : base(ChunkID)
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

    public FrontendProjectChunk(string name, uint version, uint resolutionX, uint resolutionY, string platform, string pagePath, string resourcePath, string screenPath) : base(ChunkID)
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
        if (!Platform.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Platform), Platform);

        if (!PagePath.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(PagePath), PagePath);

        if (!ResourcePath.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ResourcePath), ResourcePath);

        if (!ScreenPath.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ScreenPath), ScreenPath);

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