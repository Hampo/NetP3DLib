using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPageChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Page;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint ResolutionX { get; set; }
    public uint ResolutionY { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(ResolutionX));
            data.AddRange(BitConverter.GetBytes(ResolutionY));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public FrontendPageChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        ResolutionX = br.ReadUInt32();
        ResolutionY = br.ReadUInt32();
    }

    public FrontendPageChunk(string name, uint version, uint resolutionX, uint resolutionY) : base(ChunkID)
    {
        Name = name;
        Version = version;
        ResolutionX = resolutionX;
        ResolutionY = resolutionY;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(ResolutionX);
        bw.Write(ResolutionY);
    }

    protected override Chunk CloneSelf() => new FrontendPageChunk(Name, Version, ResolutionX, ResolutionY);
}
