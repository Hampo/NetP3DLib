using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Intersect_Mesh)]
public class IntersectMeshChunk : NamedChunk
{
    public uint NumMeshes => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Intersect_Mesh_2).Count();

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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public IntersectMeshChunk(BinaryReader br) : base((uint)ChunkIdentifier.Intersect_Mesh)
    {
        Name = br.ReadP3DString();
        var numMeshes = br.ReadUInt32();
    }

    public IntersectMeshChunk(string name) : base((uint)ChunkIdentifier.Intersect_Mesh)
    {
        Name = name;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumMeshes);
    }
}