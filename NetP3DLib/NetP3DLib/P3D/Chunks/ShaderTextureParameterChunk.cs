using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderTextureParameterChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader_Texture_Parameter;

    private string _value = string.Empty;
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value)
                return;

            _value = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => 4 + BinaryExtensions.GetP3DStringLength(Value);

    public ShaderTextureParameterChunk(BinaryReader br) : base(ChunkID)
    {
        Param = br.ReadFourCC();
        Value = br.ReadP3DString();
    }

    public ShaderTextureParameterChunk(string param, string value) : base(ChunkID)
    {
        Param = param;
        Value = value;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (!Value.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Value), Value);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteFourCC(Param);
        bw.WriteP3DString(Value);
    }

    protected override Chunk CloneSelf() => new ShaderTextureParameterChunk(Param, Value);
}