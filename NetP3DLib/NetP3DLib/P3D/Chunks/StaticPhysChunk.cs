using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Static_Phys)]
public class StaticPhysChunk : NamedChunk
{
    public uint Version { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    public StaticPhysChunk(BinaryReader br) : base((uint)ChunkIdentifier.Static_Phys)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
    }

    public StaticPhysChunk(string name, uint version) : base((uint)ChunkIdentifier.Static_Phys)
    {
        Name = name;
        Version = version;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
    }
}