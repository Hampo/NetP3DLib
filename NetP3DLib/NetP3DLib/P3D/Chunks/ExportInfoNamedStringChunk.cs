using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExportInfoNamedStringChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Export_Info_Named_String;

    private string _value = string.Empty;
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value)
                return;

            _value = value;
            RecalculateSize();
        }
    }

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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(Value);

    public ExportInfoNamedStringChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Value = br.ReadP3DString();
    }

    public ExportInfoNamedStringChunk(string name, string value) : base(ChunkID)
    {
        Name = name;
        Value = value;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Value.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Value), Value);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Value);
    }

    protected override Chunk CloneSelf() => new ExportInfoNamedStringChunk(Name, Value);
}