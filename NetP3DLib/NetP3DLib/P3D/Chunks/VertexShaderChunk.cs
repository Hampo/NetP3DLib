using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VertexShaderChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Vertex_Shader;
    
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

    public VertexShaderChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
    }

    public VertexShaderChunk(string vertextShaderName) : base(ChunkID)
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