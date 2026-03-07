using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendStringTextBibleChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_String_Text_Bible;

    private readonly P3DString _bibleName;
    public string BibleName
    {
        get => _bibleName?.Value ?? string.Empty;
        set => _bibleName.Value = value;
    }
    private readonly P3DString _stringID;
    public string StringID
    {
        get => _stringID?.Value ?? string.Empty;
        set => _stringID.Value = value;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(BibleName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(StringID));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(BibleName) + BinaryExtensions.GetP3DStringLength(StringID);

    public FrontendStringTextBibleChunk(BinaryReader br) : base(ChunkID)
    {
        _bibleName = new(this, br);
        _stringID = new(this, br);
    }

    public FrontendStringTextBibleChunk(string bibleName, string stringID) : base(ChunkID)
    {
        _bibleName = new(this, bibleName);
        _stringID = new(this, stringID);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!BibleName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(BibleName), BibleName);

        if (!StringID.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(StringID), StringID);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(BibleName);
        bw.WriteP3DString(StringID);
    }

    protected override Chunk CloneSelf() => new FrontendStringTextBibleChunk(BibleName, StringID);

    public override string ToString() => $"\"{StringID}\" ({GetChunkType(this)} (0x{ID:X}))";
}