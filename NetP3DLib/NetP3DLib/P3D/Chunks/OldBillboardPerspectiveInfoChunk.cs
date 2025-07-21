using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardPerspectiveInfoChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Perspective_Info;
    
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

    public OldBillboardPerspectiveInfoChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Perspective = br.ReadUInt32();
    }

    public OldBillboardPerspectiveInfoChunk(uint version, uint perspective) : base(ChunkID)
    {
        Version = version;
        Perspective = perspective;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Perspective);
    }
}