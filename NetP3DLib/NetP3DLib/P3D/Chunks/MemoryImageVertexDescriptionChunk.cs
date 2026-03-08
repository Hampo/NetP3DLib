using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageVertexDescriptionChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Vertex_Description;
    
    public uint Version { get; set; }
    public uint Param { get; set; }
    public uint DescriptionSize
    {
        get => (uint)(Description?.Count ?? 0);
        set
        {
            if (value == DescriptionSize)
                return;

            if (value < DescriptionSize)
            {
                while (DescriptionSize > value)
                    Description.RemoveAt(Description.Count - 1);
            }
            else
            {
                while (DescriptionSize < value)
                    Description.Add(default);
            }
        }
    }
    public SizeAwareList<byte> Description { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Param));
            data.AddRange(BitConverter.GetBytes(DescriptionSize));
            data.AddRange(Description);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + DescriptionSize;

    public MemoryImageVertexDescriptionChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadUInt32();
        int numVertices = br.ReadInt32();
        Description = CreateSizeAwareList(br.ReadBytes(numVertices));
    }

    public MemoryImageVertexDescriptionChunk(uint version, uint param, IList<byte> description) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Description = CreateSizeAwareList(description);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(DescriptionSize);
        bw.Write([..Description]);
    }

    protected override Chunk CloneSelf() => new MemoryImageVertexDescriptionChunk(Version, Param, Description);
}