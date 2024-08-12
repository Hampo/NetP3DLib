using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Multi_Controller_Track)]
public class MultiControllerTrackChunk : NamedChunk
{
    public uint Version { get; set; }
    public string Type { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Type));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + 4;

    public MultiControllerTrackChunk(BinaryReader br) : base((uint)ChunkIdentifier.Multi_Controller_Track)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadFourCC();
    }

    public MultiControllerTrackChunk(uint version, string name, string type) : base((uint)ChunkIdentifier.Multi_Controller_Track)
    {
        Version = version;
        Name = name;
        Type = type;
    }

    public override void Validate()
    {
        if (Type == null || Type.Length == 0)
            throw new InvalidDataException($"{nameof(Type)} must be at least 1 char.");

        if (Type.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(Type)} is 4 chars.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(Type);
    }
}