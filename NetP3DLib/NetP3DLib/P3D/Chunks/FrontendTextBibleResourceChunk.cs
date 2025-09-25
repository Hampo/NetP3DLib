using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendTextBibleResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Text_Bible_Resource;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Filename) + BinaryExtensions.GetP3DStringLength(InventoryName);

    public FrontendTextBibleResourceChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Filename = br.ReadP3DString();
        InventoryName = br.ReadP3DString();
    }

    public FrontendTextBibleResourceChunk(string name, uint version, string filename, string inventoryName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Filename = filename;
        InventoryName = inventoryName;
    }

    public override void Validate()
    {
        if (!Filename.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Filename), Filename);

        if (!InventoryName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(InventoryName), InventoryName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
    }

    internal override Chunk CloneSelf() => new FrontendTextBibleResourceChunk(Name, Version, Filename, InventoryName);
}