using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExportInfoNamedIntegerChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Export_Info_Named_Integer;
    
    public uint Value { get; set; }

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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    public ExportInfoNamedIntegerChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Value = br.ReadUInt32();
    }

    public ExportInfoNamedIntegerChunk(string name, uint value) : base(ChunkID)
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