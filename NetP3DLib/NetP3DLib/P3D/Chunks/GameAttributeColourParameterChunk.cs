using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Game_Attribute_Colour_Parameter)]
public class GameAttributeColourParameterChunk : NamedChunk
{
    public Color Value { get; set; }

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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    public GameAttributeColourParameterChunk(BinaryReader br) : base((uint)ChunkIdentifier.Game_Attribute_Colour_Parameter)
    {
        Name = br.ReadP3DString();
        Value = br.ReadColor();
    }

    public GameAttributeColourParameterChunk(string name, Color value) : base((uint)ChunkIdentifier.Game_Attribute_Colour_Parameter)
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