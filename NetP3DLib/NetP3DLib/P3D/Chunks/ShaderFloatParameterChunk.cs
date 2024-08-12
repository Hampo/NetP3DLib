using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Shader_Float_Parameter)]
public class ShaderFloatParameterChunk : ParamChunk
{
    public float Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => 4 + sizeof(float);

    public ShaderFloatParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Shader_Float_Parameter)
    {
        Param = br.ReadFourCC();
        Value = br.ReadSingle();
    }

    public ShaderFloatParameterChunk(string param, float value) : base((uint)ChunkIdentifier.Shader_Float_Parameter)
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