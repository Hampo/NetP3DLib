using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendProjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Project;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    public uint ResolutionX { get; set; }
    public uint ResolutionY { get; set; }
    private string _platform = string.Empty;
    public string Platform
    {
        get => _platform;
        set
        {
            if (_platform == value)
                return;

            _platform = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _pagePath = string.Empty;
    public string PagePath
    {
        get => _pagePath;
        set
        {
            if (_pagePath == value)
                return;

            _pagePath = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _resourcePath = string.Empty;
    public string ResourcePath
    {
        get => _resourcePath;
        set
        {
            if (_resourcePath == value)
                return;

            _resourcePath = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _screenPath = string.Empty;
    public string ScreenPath
    {
        get => _screenPath;
        set
        {
            if (_screenPath == value)
                return;

            _screenPath = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }

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

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Platform.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Platform), Platform);

        if (!PagePath.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(PagePath), PagePath);

        if (!ResourcePath.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ResourcePath), ResourcePath);

        if (!ScreenPath.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ScreenPath), ScreenPath);
    }

    protected override void WriteData(BinaryWriter bw)
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

    protected override Chunk CloneSelf() => new FrontendProjectChunk(Name, Version, ResolutionX, ResolutionY, Platform, PagePath, ResourcePath, ScreenPath);
}
