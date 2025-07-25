using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IntersectMeshChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Intersect_Mesh;
    
    public uint NumMeshes => GetChildCount(ChunkIdentifier.Intersect_Mesh_2);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumMeshes));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public IntersectMeshChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numMeshes = br.ReadUInt32();
    }

    public IntersectMeshChunk(string name) : base(ChunkID)
    {
        Name = name;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumMeshes);
    }

    internal override Chunk CloneSelf() => new IntersectMeshChunk(Name);
}