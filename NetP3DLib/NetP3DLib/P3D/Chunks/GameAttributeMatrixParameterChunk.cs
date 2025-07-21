using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class GameAttributeMatrixParameterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Game_Attribute_Matrix_Parameter;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(float) * 16;

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

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Value);
    }
}