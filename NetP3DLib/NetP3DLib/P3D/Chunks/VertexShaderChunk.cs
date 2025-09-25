using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VertexShaderChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vertex_Shader;
    
    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name);

    public VertexShaderChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
    }

    public VertexShaderChunk(string name) : base(ChunkID)
    {
        Name = name;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
    }

    internal override Chunk CloneSelf() => new VertexShaderChunk(Name);
}