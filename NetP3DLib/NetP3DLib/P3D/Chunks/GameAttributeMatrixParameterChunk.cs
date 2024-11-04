using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class GameAttributeMatrixParameterChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Game_Attribute_Matrix_Parameter;
    
    public Matrix4x4 Value { get; set; }

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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float) * 16;

    public GameAttributeMatrixParameterChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Value = br.ReadMatrix4x4();
    }

    public GameAttributeMatrixParameterChunk(string name, Matrix4x4 value) : base(ChunkID)
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