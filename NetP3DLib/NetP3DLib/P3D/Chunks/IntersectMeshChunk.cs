using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Intersect_Mesh)]
public class IntersectMeshChunk : NamedChunk
{
    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length;

    public IntersectMeshChunk(BinaryReader br) : base((uint)ChunkIdentifier.Intersect_Mesh)
    {
        Name = br.ReadP3DString();
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
    }
}