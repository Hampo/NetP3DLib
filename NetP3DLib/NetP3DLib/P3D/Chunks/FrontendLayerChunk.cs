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
public class FrontendLayerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Layer;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    private uint visible;
    public bool Visible
    {
        get => visible != 0;
        set => visible = value ? 1u : 0u;
    }
    private uint editable;
    public bool Editable
    {
        get => editable != 0;
        set => editable = value ? 1u : 0u;
    }
    public uint Alpha { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(visible));
            data.AddRange(BitConverter.GetBytes(editable));
            data.AddRange(BitConverter.GetBytes(Alpha));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public FrontendLayerChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        visible = br.ReadUInt32();
        editable = br.ReadUInt32();
        Alpha = br.ReadUInt32();
    }

    public FrontendLayerChunk(string name, uint version, bool visible, bool editable, uint alpha) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Visible = visible;
        Editable = editable;
        Alpha = alpha;
    }

    public override void Validate()
    {
        if (Alpha > 256)
            throw new InvalidP3DException(this, $"{nameof(Alpha)} must be between 0 and 256.");

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(visible);
        bw.Write(editable);
        bw.Write(Alpha);
    }

    protected override Chunk CloneSelf() => new FrontendLayerChunk(Name, Version, Visible, Editable, Alpha);
}
