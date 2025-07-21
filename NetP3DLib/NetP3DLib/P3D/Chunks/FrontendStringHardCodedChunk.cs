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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(String);

    public FrontendStringHardCodedChunk(BinaryReader br) : base(ChunkID)
    {
        String = br.ReadP3DString();
    }

    public FrontendStringHardCodedChunk(string @string) : base(ChunkID)
    {
        String = @string;
    }

    public override void Validate()
    {
        if (!String.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(String), String);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(String);
    }
}