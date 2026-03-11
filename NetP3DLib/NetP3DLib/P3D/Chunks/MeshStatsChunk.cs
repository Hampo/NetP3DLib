using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MeshStatsChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Mesh_Stats;

    public uint Version { get; set; }
    public uint IsRendered { get; set; }
    public uint IsCollision { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(IsRendered));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public MeshStatsChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadP3DString(), br.ReadUInt32(), br.ReadUInt32())
    {
    }

    public MeshStatsChunk(uint version, string name, uint isRendered, uint isCollision) : base(ChunkID, name)
    {
        Version = version;
        IsRendered = isRendered;
        IsCollision = isCollision;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(IsRendered);
        bw.Write(IsCollision);
    }

    protected override Chunk CloneSelf() => new MeshStatsChunk(Version, Name, IsRendered, IsCollision);
}