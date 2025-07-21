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
    
    public string BibleName { get; set; }
    public string StringID { get; set; }

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

    public override void Validate()
    {
        if (!BibleName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(BibleName), BibleName);

        if (!StringID.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(StringID), StringID);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(BibleName);
        bw.WriteP3DString(StringID);
    }
}