using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Export_Info)]
public class ExportInfoChunk : NamedChunk
{
    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length;

    public ExportInfoChunk(BinaryReader br) : base((uint)ChunkIdentifier.Export_Info)
    {
        Name = br.ReadP3DString();
    }

    public ExportInfoChunk(string name) : base((uint)ChunkIdentifier.Export_Info)
    {
        Name = name;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
    }
}