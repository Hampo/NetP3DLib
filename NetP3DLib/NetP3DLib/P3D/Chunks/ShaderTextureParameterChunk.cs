using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Shader_Texture_Parameter)]
public class ShaderTextureParameterChunk : ParamChunk
{
    public string Value { get; set; }

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
    public override uint DataLength => 4 + (uint)BinaryExtensions.GetP3DStringBytes(Value).Length;

    public ShaderTextureParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Shader_Texture_Parameter)
    {
        Param = br.ReadFourCC();
        Value = br.ReadP3DString();
    }

    public ShaderTextureParameterChunk(string param, string value) : base((uint)ChunkIdentifier.Shader_Texture_Parameter)
    {
        Param = param;
        Value = value;
    }

    public override void Validate()
    {
        if (Value == null)
            throw new InvalidDataException($"{nameof(Value)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Value).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Value)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteFourCC(Param);
        bw.WriteP3DString(Value);
    }
}