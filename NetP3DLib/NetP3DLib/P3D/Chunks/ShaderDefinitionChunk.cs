using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderDefinitionChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader_Definition;

    private string _definition = string.Empty;
    public string Definition
    {
        get => _definition;
        set
        {
            if (_definition == value)
                return;

            _definition = value;
            RecalculateSize();
        }
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
        Name = br.ReadP3DString();
        Definition = br.ReadP3DLongString();
    }

    public ShaderDefinitionChunk(string name, string definition) : base(ChunkID)
    {
        Name = name;
        Definition = definition;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DLongString(Definition);
    }

    protected override Chunk CloneSelf() => new ShaderDefinitionChunk(Name, Definition);
}