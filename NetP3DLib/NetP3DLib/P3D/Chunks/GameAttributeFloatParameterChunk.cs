using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Game_Attribute_Float_Parameter)]
public class GameAttributeFloatParameterChunk : NamedChunk
{
    public float Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float);

    public GameAttributeFloatParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Game_Attribute_Float_Parameter)
    {
        Name = br.ReadP3DString();
        Value = br.ReadSingle();
    }

    public GameAttributeFloatParameterChunk(string name, float value) : base((uint)ChunkIdentifier.Game_Attribute_Float_Parameter)
    {
        Name = name;
        Value = value;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Value);
    }
}