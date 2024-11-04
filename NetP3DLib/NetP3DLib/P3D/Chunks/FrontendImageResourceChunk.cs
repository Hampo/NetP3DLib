using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendImageResourceChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Image_Resource;
    
    public uint Version { get; set; }
    public string Filename { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Filename));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Filename).Length;

    public FrontendImageResourceChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Filename = br.ReadP3DString();
    }

    public FrontendImageResourceChunk(string name, uint version, string filename) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Filename = filename;
    }

    public override void Validate()
    {
        if (Filename == null)
            throw new InvalidDataException($"{nameof(Filename)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Filename).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Filename)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(Filename);
    }
}