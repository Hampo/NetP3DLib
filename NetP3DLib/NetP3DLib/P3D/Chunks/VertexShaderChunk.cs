using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Vertex_Shader)]
public class VertexShaderChunk : Chunk
{
    public string VertexShaderName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(VertexShaderName));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(VertexShaderName).Length;

    public VertexShaderChunk(BinaryReader br) : base((uint)ChunkIdentifier.Vertex_Shader)
    {
        VertexShaderName = br.ReadP3DString();
    }

    public VertexShaderChunk(string vertextShaderName) : base((uint)ChunkIdentifier.Vertex_Shader)
    {
        VertexShaderName = vertextShaderName;
    }

    public override void Validate()
    {
        if (VertexShaderName == null)
            throw new InvalidDataException($"{nameof(VertexShaderName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(VertexShaderName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(VertexShaderName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(VertexShaderName);
    }
}