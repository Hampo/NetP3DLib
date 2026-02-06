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
public class FrontendPure3DResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Pure3D_Resource;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    private string _filename = string.Empty;
    public string Filename
    {
        get => _filename;
        set
        {
            if (_filename == value)
                return;

            _filename = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _inventoryName = string.Empty;
    public string InventoryName
    {
        get => _inventoryName;
        set
        {
            if (_inventoryName == value)
                return;

            _inventoryName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _cameraName = string.Empty;
    public string CameraName
    {
        get => _cameraName;
        set
        {
            if (_cameraName == value)
                return;

            _cameraName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _animationName = string.Empty;
    public string AnimationName
    {
        get => _animationName;
        set
        {
            if (_animationName == value)
                return;

            _animationName = value;
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
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Filename));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(InventoryName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(CameraName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(AnimationName));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Filename) + BinaryExtensions.GetP3DStringLength(InventoryName) + BinaryExtensions.GetP3DStringLength(CameraName) + BinaryExtensions.GetP3DStringLength(AnimationName);

    public FrontendPure3DResourceChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Filename = br.ReadP3DString();
        InventoryName = br.ReadP3DString();
        CameraName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public FrontendPure3DResourceChunk(string name, uint version, string filename, string inventoryName, string cameraName, string animationName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Filename = filename;
        InventoryName = inventoryName;
        CameraName = cameraName;
        AnimationName = animationName;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (!Filename.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Filename), Filename);

        if (!InventoryName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(InventoryName), InventoryName);

        if (!CameraName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(CameraName), CameraName);

        if (!AnimationName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(AnimationName), AnimationName);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
        bw.WriteP3DString(CameraName);
        bw.WriteP3DString(AnimationName);
    }

    protected override Chunk CloneSelf() => new FrontendPure3DResourceChunk(Name, Version, Filename, InventoryName, CameraName, AnimationName);
}
