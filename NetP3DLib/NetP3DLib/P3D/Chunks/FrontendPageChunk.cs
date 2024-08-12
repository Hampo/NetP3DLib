using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Frontend_Page)]
public class FrontendPageChunk : NamedChunk
{
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public FrontendPageChunk(BinaryReader br) : base((uint)ChunkIdentifier.Frontend_Page)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        ResolutionX = br.ReadUInt32();
        ResolutionY = br.ReadUInt32();
    }

    public FrontendPageChunk(string name, uint version, uint resolutionX, uint resolutionY) : base((uint)ChunkIdentifier.Frontend_Page)
    {
        Name = name;
        Version = version;
        ResolutionX = resolutionX;
        ResolutionY = resolutionY;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(ResolutionX);
        bw.Write(ResolutionY);
    }
}