using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendTextBibleChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_Text_Bible;
    
    public uint NumLanguages => (uint)Languages.Length;
    public string Languages => string.Concat(GetChunksOfType<FrontendLanguageChunk>().Select(x => x.Language));

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumLanguages));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Languages));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Languages).Length;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public FrontendTextBibleChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numLanguages = br.ReadUInt32();
        var languages = br.ReadP3DString();
    }

    public FrontendTextBibleChunk(string name) : base(ChunkID)
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
        bw.Write(NumLanguages);
        bw.WriteP3DString(Languages);
    }
}