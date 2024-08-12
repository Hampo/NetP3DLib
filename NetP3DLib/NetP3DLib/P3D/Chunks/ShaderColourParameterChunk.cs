using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Shader_Colour_Parameter)]
public class ShaderColourParameterChunk : ParamChunk
{
    public Color Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BinaryExtensions.GetBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => 4 + sizeof(uint);

    public ShaderColourParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Shader_Colour_Parameter)
    {
        Param = br.ReadFourCC();
        Value = br.ReadColor();
    }

    public ShaderColourParameterChunk(string param, Color value) : base((uint)ChunkIdentifier.Shader_Colour_Parameter)
    {
        Param = param;
        Value = value;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteFourCC(Param);
        bw.Write(Value);
    }
}