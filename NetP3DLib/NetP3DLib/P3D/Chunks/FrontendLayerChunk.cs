using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendLayerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Layer;
    
    public uint Version { get; set; }
    public uint Visible { get; set; }
    public uint Editable { get; set; }
    public uint Alpha { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Visible));
            data.AddRange(BitConverter.GetBytes(Editable));
            data.AddRange(BitConverter.GetBytes(Alpha));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public FrontendLayerChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Visible = br.ReadUInt32();
        Editable = br.ReadUInt32();
        Alpha = br.ReadUInt32();
    }

    public FrontendLayerChunk(string name, uint version, uint visible, uint editable, uint alpha) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Visible = visible;
        Editable = editable;
        Alpha = alpha;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Visible);
        bw.Write(Editable);
        bw.Write(Alpha);
    }

    internal override Chunk CloneSelf() => new FrontendLayerChunk(Name, Version, Visible, Editable, Alpha);
}