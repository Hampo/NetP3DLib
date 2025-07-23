using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class WorldSphereChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.World_Sphere;
    
    public uint Version { get; set; }
    public uint NumMeshes => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Mesh).Count();
    public uint NumOldBillboardQuadGroups => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Billboard_Quad_Group).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumMeshes));
            data.AddRange(BitConverter.GetBytes(NumOldBillboardQuadGroups));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public WorldSphereChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numMeshes = br.ReadUInt32();
        var numOldBillboardQuadGroups = br.ReadUInt32();
    }

    public WorldSphereChunk(string name, uint version) : base(ChunkID)
    {
        Name = name;
        Version = version;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumMeshes);
        bw.Write(NumOldBillboardQuadGroups);
    }

    internal override Chunk CloneSelf() => new WorldSphereChunk(Name, Version);
}