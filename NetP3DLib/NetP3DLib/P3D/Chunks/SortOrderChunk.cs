using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SortOrderChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Sort_Order;
    
    public uint Version { get; set; }
    public float SortOrder { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(SortOrder));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public SortOrderChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        SortOrder = br.ReadSingle();
    }

    public SortOrderChunk(uint version, float sortOrder) : base(ChunkID)
    {
        Version = version;
        SortOrder = sortOrder;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(SortOrder);
    }

    internal override Chunk CloneSelf() => new SortOrderChunk(Version, SortOrder);
}