using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Frontend_Text_Style_Resource)]
public class FrontendTextStyleResourceChunk : NamedChunk
{
    public uint Version { get; set; }
    public string Filename { get; set; }
    public string InventoryName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Filename));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(InventoryName));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Filename).Length + (uint)BinaryExtensions.GetP3DStringBytes(InventoryName).Length;

    public FrontendTextStyleResourceChunk(BinaryReader br) : base((uint)ChunkIdentifier.Frontend_Text_Style_Resource)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Filename = br.ReadP3DString();
        InventoryName = br.ReadP3DString();
    }

    public FrontendTextStyleResourceChunk(string name, uint version, string filename, string inventoryName) : base((uint)ChunkIdentifier.Frontend_Text_Style_Resource)
    {
        Name = name;
        Version = version;
        Filename = filename;
        InventoryName = inventoryName;
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

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
    }
}