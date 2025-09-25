using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardPerspectiveInfoChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Perspective_Info;
    
    public uint Version { get; set; }
    private uint perspectiveScale;
    public bool PerspectiveScale
    {
        get => perspectiveScale != 0;
        set => perspectiveScale = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(perspectiveScale));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public OldBillboardPerspectiveInfoChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        perspectiveScale = br.ReadUInt32();
    }

    public OldBillboardPerspectiveInfoChunk(uint version, bool perspectiveScale) : base(ChunkID)
    {
        Version = version;
        PerspectiveScale = perspectiveScale;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(perspectiveScale);
    }

    internal override Chunk CloneSelf() => new OldBillboardPerspectiveInfoChunk(Version, PerspectiveScale);
}