using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Vertex_Anim_Key_Frame)]
public class VertexAnimKeyFrameChunk : Chunk
{
    public uint Version { get; set; }
    public uint KeyFrameId { get; set; }
    public uint PrimGroupIndex { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(KeyFrameId));
            data.AddRange(BitConverter.GetBytes(PrimGroupIndex));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint);

    public VertexAnimKeyFrameChunk(BinaryReader br) : base((uint)ChunkIdentifier.Vertex_Anim_Key_Frame)
    {
        Version = br.ReadUInt32();
        KeyFrameId = br.ReadUInt32();
        PrimGroupIndex = br.ReadUInt32();
    }

    public VertexAnimKeyFrameChunk(uint version, uint keyFrameId, uint primGroupIndex) : base((uint)ChunkIdentifier.Vertex_Anim_Key_Frame)
    {
        Version = version;
        KeyFrameId = keyFrameId;
        PrimGroupIndex = primGroupIndex;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(KeyFrameId);
        bw.Write(PrimGroupIndex);
    }
}