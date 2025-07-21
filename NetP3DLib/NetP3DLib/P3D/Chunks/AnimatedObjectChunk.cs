using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimatedObjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animated_Object;
    
    public uint Version { get; set; }
    public string FactoryName { get; set; }
    public uint StartingAnimation { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(FactoryName));
            data.AddRange(BitConverter.GetBytes(StartingAnimation));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(FactoryName) + sizeof(uint);

    public AnimatedObjectChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FactoryName = br.ReadP3DString();
        StartingAnimation = br.ReadUInt32();
    }

    public AnimatedObjectChunk(uint version, string name, string factoryName, uint startingAnimation) : base(ChunkID)
    {
        Version = version;
        Name = name;
        FactoryName = factoryName;
        StartingAnimation = startingAnimation;
    }

    public override void Validate()
    {
        if (!FactoryName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(FactoryName), FactoryName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(FactoryName);
        bw.Write(StartingAnimation);
    }
}