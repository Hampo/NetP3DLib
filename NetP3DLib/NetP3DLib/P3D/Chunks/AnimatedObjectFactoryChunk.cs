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
public class AnimatedObjectFactoryChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animated_Object_Factory;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    private string _baseAnimation = string.Empty;
    public string BaseAnimation
    {
        get => _baseAnimation;
        set
        {
            if (_baseAnimation == value)
                return;

            _baseAnimation = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
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

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!BaseAnimation.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(BaseAnimation), BaseAnimation);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(BaseAnimation);
        bw.Write(NumAnimations);
    }

    protected override Chunk CloneSelf() => new AnimatedObjectFactoryChunk(Version, Name, BaseAnimation, NumAnimations);
}
