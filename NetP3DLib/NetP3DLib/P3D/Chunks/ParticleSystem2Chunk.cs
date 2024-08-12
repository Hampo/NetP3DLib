using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Particle_System_2)]
public class ParticleSystem2Chunk : NamedChunk
{
    public uint Version { get; set; }
    public string FactoryName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(FactoryName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(FactoryName).Length;

    public ParticleSystem2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Particle_System_2)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FactoryName = br.ReadP3DString();
    }

    public ParticleSystem2Chunk(uint version, string name, string factoryName) : base((uint)ChunkIdentifier.Particle_System_2)
    {
        Version = version;
        Name = name;
        FactoryName = factoryName;
    }

    public override void Validate()
    {
        if (FactoryName == null)
            throw new InvalidDataException($"{nameof(FactoryName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(FactoryName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(FactoryName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(FactoryName);
    }
}