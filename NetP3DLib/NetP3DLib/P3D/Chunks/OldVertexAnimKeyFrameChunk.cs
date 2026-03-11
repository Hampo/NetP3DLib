using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldVertexAnimKeyFrameChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Vertex_Anim_Key_Frame;

    [DefaultValue(0)]
    public uint Version { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name);

    public OldVertexAnimKeyFrameChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadP3DString())
    {
    }

    public OldVertexAnimKeyFrameChunk(uint version, string name) : base(ChunkID, name)
    {
        Version = version;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
    }

    protected override Chunk CloneSelf() => new OldVertexAnimKeyFrameChunk(Version, Name);
}
