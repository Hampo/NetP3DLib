using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

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

    public MeshStatsChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        IsRendered = br.ReadUInt32();
        IsCollision = br.ReadUInt32();
    }

    public MeshStatsChunk(uint version, string name, uint isRendered, uint isCollision) : base(ChunkID)
    {
        Version = version;
        Name = name;
        IsRendered = isRendered;
        IsCollision = isCollision;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(IsRendered);
        bw.Write(IsCollision);
    }
}