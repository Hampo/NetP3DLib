using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardPerspectiveInfoChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Perspective_Info;

    [DefaultValue(0)]
    public uint Version { get; set; }
    private uint _perspectiveScale;
    public bool PerspectiveScale
    {
        get => _perspectiveScale != 0;
        set => _perspectiveScale = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(_perspectiveScale));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    public OldBillboardPerspectiveInfoChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadUInt32())
    {
    }

    public OldBillboardPerspectiveInfoChunk(uint version, bool perspectiveScale) : this(version, perspectiveScale ? 1u : 0u)
    {
    }

    public OldBillboardPerspectiveInfoChunk(uint version, uint perspectiveScale) : base(ChunkID)
    {
        Version = version;
        _perspectiveScale = perspectiveScale;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(_perspectiveScale);
    }

    protected override Chunk CloneSelf() => new OldBillboardPerspectiveInfoChunk(Version, PerspectiveScale);
}
