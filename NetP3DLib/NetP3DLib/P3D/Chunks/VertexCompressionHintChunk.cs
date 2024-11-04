using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Vertex_Compression_Hint)]
public class VertexCompressionHintChunk : Chunk
{
    public uint Version { get; set; }
    public uint UV0Size { get; set; }
    public uint UV1Size { get; set; }
    public uint UV2Size { get; set; }
    public uint UV3Size { get; set; }
    public uint NormalSize { get; set; }
    public uint ColourSize { get; set; }
    public uint PositionSize { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(UV0Size));
            data.AddRange(BitConverter.GetBytes(UV1Size));
            data.AddRange(BitConverter.GetBytes(UV2Size));
            data.AddRange(BitConverter.GetBytes(UV3Size));
            data.AddRange(BitConverter.GetBytes(NormalSize));
            data.AddRange(BitConverter.GetBytes(ColourSize));
            data.AddRange(BitConverter.GetBytes(PositionSize));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public VertexCompressionHintChunk(BinaryReader br) : base((uint)ChunkIdentifier.Vertex_Compression_Hint)
    {
        Version = br.ReadUInt32();
        UV0Size = br.ReadUInt32();
        UV1Size = br.ReadUInt32();
        UV2Size = br.ReadUInt32();
        UV3Size = br.ReadUInt32();
        NormalSize = br.ReadUInt32();
        ColourSize = br.ReadUInt32();
        PositionSize = br.ReadUInt32();
    }

    public VertexCompressionHintChunk(uint version, uint uv0Size, uint uv1Size, uint uv2Size, uint uv3Size, uint normalSize, uint colourSize, uint positionSize) : base((uint)ChunkIdentifier.Vertex_Compression_Hint)
    {
        Version = version;
        UV0Size = uv0Size;
        UV1Size = uv1Size;
        UV2Size = uv2Size;
        UV3Size = uv3Size;
        NormalSize = normalSize;
        ColourSize = colourSize;
        PositionSize = positionSize;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(UV0Size);
        bw.Write(UV1Size);
        bw.Write(UV2Size);
        bw.Write(UV3Size);
        bw.Write(NormalSize);
        bw.Write(ColourSize);
        bw.Write(PositionSize);
    }
}