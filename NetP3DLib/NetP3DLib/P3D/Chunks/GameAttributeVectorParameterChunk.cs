using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Game_Attribute_Vector_Parameter)]
public class GameAttributeVectorParameterChunk : NamedChunk
{
    public Vector3 Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float) * 3;

    public GameAttributeVectorParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Game_Attribute_Vector_Parameter)
    {
        Name = br.ReadP3DString();
        Value = br.ReadVector3();
    }

    public GameAttributeVectorParameterChunk(string name, Vector3 value) : base((uint)ChunkIdentifier.Game_Attribute_Vector_Parameter)
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