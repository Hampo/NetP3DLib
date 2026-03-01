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
public class FrontendTextStyleResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Text_Style_Resource;
    
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

    public FrontendTextStyleResourceChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Filename = br.ReadP3DString();
        InventoryName = br.ReadP3DString();
    }

    public FrontendTextStyleResourceChunk(string name, uint version, string filename, string inventoryName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Filename = filename;
        InventoryName = inventoryName;
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

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
        bw.WriteP3DString(InventoryName);
    }

    protected override Chunk CloneSelf() => new FrontendTextStyleResourceChunk(Name, Version, Filename, InventoryName);
}
