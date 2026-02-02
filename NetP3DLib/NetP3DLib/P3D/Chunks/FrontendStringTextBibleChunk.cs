using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendStringTextBibleChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_String_Text_Bible;

    private string _bibleName = string.Empty;
    public string BibleName
    {
        get => _bibleName;
        set
        {
            if (_bibleName == value)
                return;

            _bibleName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _stringID = string.Empty;
    public string StringID
    {
        get => _stringID;
        set
        {
            if (_stringID == value)
                return;

            _stringID = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
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
        BibleName = br.ReadP3DString();
        StringID = br.ReadP3DString();
    }

    public FrontendStringTextBibleChunk(string bibleName, string stringID) : base(ChunkID)
    {
        BibleName = bibleName;
        StringID = stringID;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!BibleName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(BibleName), BibleName);

        if (!StringID.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(StringID), StringID);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(BibleName);
        bw.WriteP3DString(StringID);
    }

    protected override Chunk CloneSelf() => new FrontendStringTextBibleChunk(BibleName, StringID);

    public override string ToString() => $"\"{StringID}\" ({GetChunkType(this)} (0x{ID:X}))";
}