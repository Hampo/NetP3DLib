using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendGroup2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Group_2;
    
    public uint Version { get; set; }
    public uint Alpha { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Alpha));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public FrontendGroup2Chunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Alpha = br.ReadUInt32();
    }

    public FrontendGroup2Chunk(string name, uint version, uint alpha) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Alpha = alpha;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (Alpha > 256)
            yield return new InvalidP3DException(this, $"{nameof(Alpha)} must be between 0 and 256.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Alpha);
    }

    protected override Chunk CloneSelf() => new FrontendGroup2Chunk(Name, Version, Alpha);
}