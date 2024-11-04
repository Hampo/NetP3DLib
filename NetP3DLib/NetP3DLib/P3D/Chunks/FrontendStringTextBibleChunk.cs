using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendStringTextBibleChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_String_Text_Bible;
    
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(BibleName).Length + (uint)BinaryExtensions.GetP3DStringBytes(StringID).Length;

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
        if (BibleName == null)
            throw new InvalidDataException($"{nameof(BibleName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(BibleName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(BibleName)} is 255 bytes.");

        if (StringID == null)
            throw new InvalidDataException($"{nameof(StringID)} cannot be null.");
        if (Encoding.UTF8.GetBytes(StringID).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(StringID)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(BibleName);
        bw.WriteP3DString(StringID);
    }
}