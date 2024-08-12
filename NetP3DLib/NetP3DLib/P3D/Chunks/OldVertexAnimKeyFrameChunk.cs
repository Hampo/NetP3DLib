using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Vertex_Anim_Key_Frame)]
public class OldVertexAnimKeyFrameChunk : NamedChunk
{
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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length;

    public OldVertexAnimKeyFrameChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Vertex_Anim_Key_Frame)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
    }

    public OldVertexAnimKeyFrameChunk(uint version, string name) : base((uint)ChunkIdentifier.Old_Vertex_Anim_Key_Frame)
    {
        Version = version;
        Name = name;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
    }
}