using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderIntegerParameterChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader_Integer_Parameter;
    
    public uint Value { get; set; }

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
    public override uint DataLength => 4 + sizeof(uint);

    public ShaderIntegerParameterChunk(BinaryReader br) : base(ChunkID)
    {
        Param = br.ReadFourCC();
        Value = br.ReadUInt32();
    }

    public ShaderIntegerParameterChunk(string param, uint value) : base(ChunkID)
    {
        Param = param;
        Value = value;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteFourCC(Param);
        bw.Write(Value);
    }

    internal override Chunk CloneSelf() => new ShaderIntegerParameterChunk(Param, Value);
}