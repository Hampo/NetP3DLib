using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendStringHardCodedChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_String_Hard_Coded;

    private string _string = string.Empty;
    public string String
    {
        get => _string;
        set
        {
            if (_string == value)
                return;

            _string = value;
            RecalculateSize();
        }
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(String));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(String);

    public FrontendStringHardCodedChunk(BinaryReader br) : base(ChunkID)
    {
        String = br.ReadP3DString();
    }

    public FrontendStringHardCodedChunk(string @string) : base(ChunkID)
    {
        String = @string;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!String.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(String), String);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(String);
    }

    protected override Chunk CloneSelf() => new FrontendStringHardCodedChunk(String);
}