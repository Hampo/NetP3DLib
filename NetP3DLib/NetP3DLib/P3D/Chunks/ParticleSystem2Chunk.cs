using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ParticleSystem2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Particle_System_2;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    private string _factoryName = string.Empty;
    public string FactoryName
    {
        get => _factoryName;
        set
        {
            if (_factoryName == value)
                return;

            _factoryName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }

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
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(FactoryName);

    public ParticleSystem2Chunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FactoryName = br.ReadP3DString();
    }

    public ParticleSystem2Chunk(uint version, string name, string factoryName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        FactoryName = factoryName;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!FactoryName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(FactoryName), FactoryName);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(FactoryName);
    }

    protected override Chunk CloneSelf() => new ParticleSystem2Chunk(Version, Name, FactoryName);
}
