using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Frontend_String_Hard_Coded)]
public class FrontendStringHardCodedChunk : Chunk
{
    public string String { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(String));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(String).Length;

    public FrontendStringHardCodedChunk(BinaryReader br) : base((uint)ChunkIdentifier.Frontend_String_Hard_Coded)
    {
        String = br.ReadP3DString();
    }

    public FrontendStringHardCodedChunk(string @string) : base((uint)ChunkIdentifier.Frontend_String_Hard_Coded)
    {
        String = @string;
    }

    public override void Validate()
    {
        if (String == null)
            throw new InvalidDataException($"{nameof(String)} cannot be null.");
        if (Encoding.UTF8.GetBytes(String).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(String)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(String);
    }
}