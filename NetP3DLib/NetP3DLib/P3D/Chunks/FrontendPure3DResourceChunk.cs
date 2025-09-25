using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPure3DResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Pure3D_Resource;
    
    public uint Version { get; set; }
    public string Filename { get; set; }
    public string InventoryName { get; set; }
    public string CameraName { get; set; }
    public string AnimationName { get; set; }

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

    public override void Validate()
    {
        if (!Filename.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Filename), Filename);

        if (!InventoryName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(InventoryName), InventoryName);

        if (!CameraName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(CameraName), CameraName);

        if (!AnimationName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(AnimationName), AnimationName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
        bw.WriteP3DString(CameraName);
        bw.WriteP3DString(AnimationName);
    }

    internal override Chunk CloneSelf() => new FrontendPure3DResourceChunk(Name, Version, Filename, InventoryName, CameraName, AnimationName);
}