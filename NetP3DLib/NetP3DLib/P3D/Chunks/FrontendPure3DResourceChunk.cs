using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPure3DResourceChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_Pure3D_Resource;
    
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Filename).Length + (uint)BinaryExtensions.GetP3DStringBytes(InventoryName).Length + (uint)BinaryExtensions.GetP3DStringBytes(CameraName).Length + (uint)BinaryExtensions.GetP3DStringBytes(AnimationName).Length;

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
        if (Filename == null)
            throw new InvalidDataException($"{nameof(Filename)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Filename).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Filename)} is 255 bytes.");

        if (InventoryName == null)
            throw new InvalidDataException($"{nameof(InventoryName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(InventoryName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(InventoryName)} is 255 bytes.");

        if (CameraName == null)
            throw new InvalidDataException($"{nameof(CameraName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(CameraName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(CameraName)} is 255 bytes.");

        if (AnimationName == null)
            throw new InvalidDataException($"{nameof(AnimationName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(AnimationName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(AnimationName)} is 255 bytes.");

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
}