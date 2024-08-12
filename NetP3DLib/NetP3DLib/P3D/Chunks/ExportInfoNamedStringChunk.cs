using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Export_Info_Named_String)]
public class ExportInfoNamedStringChunk : NamedChunk
{
    public string Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(Value).Length;

    public ExportInfoNamedStringChunk(BinaryReader br) : base((uint)ChunkIdentifier.Export_Info_Named_String)
    {
        Name = br.ReadP3DString();
        Value = br.ReadP3DString();
    }

    public ExportInfoNamedStringChunk(string name, string value) : base((uint)ChunkIdentifier.Export_Info_Named_String)
    {
        Name = name;
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
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Value);
    }
}