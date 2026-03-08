using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendTextBibleResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Text_Bible_Resource;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    private readonly P3DString _filename;
    public string Filename
    {
        get => _filename?.Value ?? string.Empty;
        set => _filename.Value = value;
    }
    private readonly P3DString _inventoryName;
    public string InventoryName
    {
        get => _inventoryName?.Value ?? string.Empty;
        set => _inventoryName.Value = value;
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

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Filename) + BinaryExtensions.GetP3DStringLength(InventoryName);

    public FrontendTextBibleResourceChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        Version = br.ReadUInt32();
        _filename = new(this, br);
        _inventoryName = new(this, br);
    }

    public FrontendTextBibleResourceChunk(string name, uint version, string filename, string inventoryName) : base(ChunkID)
    {
        _name = new(this, name);
        Version = version;
        _filename = new(this, filename);
        _inventoryName = new(this, inventoryName);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Filename.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Filename), Filename);

        if (!InventoryName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(InventoryName), InventoryName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
    }

    protected override Chunk CloneSelf() => new FrontendTextBibleResourceChunk(Name, Version, Filename, InventoryName);
}
