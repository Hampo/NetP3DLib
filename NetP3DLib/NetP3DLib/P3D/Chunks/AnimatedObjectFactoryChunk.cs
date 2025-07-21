using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimatedObjectFactoryChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animated_Object_Factory;
    
    public uint Version { get; set; }
    public string BaseAnimation { get; set; }
    public uint NumAnimations { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(BaseAnimation));
            data.AddRange(BitConverter.GetBytes(NumAnimations));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(BaseAnimation) + sizeof(uint);

    public AnimatedObjectFactoryChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        BaseAnimation = br.ReadP3DString();
        NumAnimations = br.ReadUInt32();
    }

    public AnimatedObjectFactoryChunk(uint version, string name, string baseAnimation, uint numAnimations) : base(ChunkID)
    {
        Version = version;
        Name = name;
        BaseAnimation = baseAnimation;
        NumAnimations = numAnimations;
    }

    public override void Validate()
    {
        if (!BaseAnimation.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(BaseAnimation), BaseAnimation);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(BaseAnimation);
        bw.Write(NumAnimations);
    }
}