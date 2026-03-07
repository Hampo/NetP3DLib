using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderDefinitionChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader_Definition;

    private readonly P3DLongString _definition;
    public string Definition
    {
        get => _definition?.Value ?? string.Empty;
        set => _definition.Value = value;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DLongStringBytes(Definition));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DLongStringLength(Definition);

    public ShaderDefinitionChunk(BinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        _definition = new(this, br);
    }

    public ShaderDefinitionChunk(string name, string definition) : base(ChunkID)
    {
        _name = new(this, name);
        _definition = new(this, definition);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DLongString(Definition);
    }

    protected override Chunk CloneSelf() => new ShaderDefinitionChunk(Name, Definition);
}