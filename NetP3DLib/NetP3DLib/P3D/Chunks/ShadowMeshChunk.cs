using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShadowMeshChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shadow_Mesh;
    
    public uint Version { get; set; }
    // TODO: Calculate from children
    public uint NumVertices { get; set; }
    public uint NumTriangles { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumVertices));
            data.AddRange(BitConverter.GetBytes(NumTriangles));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public ShadowMeshChunk(BinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        Version = br.ReadUInt32();
        NumVertices = br.ReadUInt32();
        NumTriangles = br.ReadUInt32();
    }

    public ShadowMeshChunk(string name, uint version, uint numVertices, uint numTriangles) : base(ChunkID)
    {
        _name = new(this, name);
        Version = version;
        NumVertices = numVertices;
        NumTriangles = numTriangles;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumVertices);
        bw.Write(NumTriangles);
    }

    protected override Chunk CloneSelf() => new ShadowMeshChunk(Name, Version, NumVertices, NumTriangles);
}