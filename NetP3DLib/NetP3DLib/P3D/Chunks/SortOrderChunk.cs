using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Sort_Order)]
public class SortOrderChunk : Chunk
{
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

    public SortOrderChunk(BinaryReader br) : base((uint)ChunkIdentifier.Sort_Order)
    {
        Version = br.ReadUInt32();
        SortOrder = br.ReadSingle();
    }

    public SortOrderChunk(uint version, float sortOrder) : base((uint)ChunkIdentifier.Sort_Order)
    {
        Version = version;
        SortOrder = sortOrder;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(SortOrder);
    }
}