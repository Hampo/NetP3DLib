using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Billboard_Perspective_Info)]
public class OldBillboardPerspectiveInfoChunk : Chunk
{
    public uint Version { get; set; }
    public uint Perspective { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Perspective));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public OldBillboardPerspectiveInfoChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Billboard_Perspective_Info)
    {
        Version = br.ReadUInt32();
        Perspective = br.ReadUInt32();
    }

    public OldBillboardPerspectiveInfoChunk(uint version, uint perspective) : base((uint)ChunkIdentifier.Old_Billboard_Perspective_Info)
    {
        Version = version;
        Perspective = perspective;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Perspective);
    }
}