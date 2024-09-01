using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Vertex_Shader)]
public class VertexShaderChunk : NamedChunk
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

    public VertexShaderChunk(BinaryReader br) : base((uint)ChunkIdentifier.Vertex_Shader)
    {
        Name = br.ReadP3DString();
    }

    public VertexShaderChunk(string vertextShaderName) : base((uint)ChunkIdentifier.Vertex_Shader)
    {
        Name = vertextShaderName;
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