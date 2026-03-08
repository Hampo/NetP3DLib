using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation;
    public enum Platform
    {
        PC,
        PS2,
        XBOX,
        GC,
    }

    [DefaultValue(0)]
    public uint Version { get; set; }
    public AnimationType AnimationType { get; set; }
    public float NumFrames { get; set; }
    public float FrameRate { get; set; }
    private uint cyclic;
    public bool Cyclic
    {
        get => cyclic == 1;
        set => cyclic = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)AnimationType));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(cyclic));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + sizeof(float) + sizeof(float) + sizeof(uint);

    public AnimationChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        AnimationType = (AnimationType)br.ReadUInt32();
        NumFrames = br.ReadSingle();
        FrameRate = br.ReadSingle();
        cyclic = br.ReadUInt32();
    }

    public AnimationChunk(uint version, string name, AnimationType animationType, float numFrames, float frameRate, bool cyclic) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        AnimationType = animationType;
        NumFrames = numFrames;
        FrameRate = frameRate;
        Cyclic = cyclic;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        var animationSizeCount = GetChildCount(ChunkIdentifier.Animation_Size);
        if (animationSizeCount > 1)
            yield return new InvalidP3DException(this, $"There can only be one {nameof(AnimationSizeChunk)} per {nameof(AnimationChunk)}.");
        var animationSize = animationSizeCount == 1 ? GetFirstChunkOfType<AnimationSizeChunk>() : null;

        var animationGroupListCount = GetChildCount(ChunkIdentifier.Animation_Group_List);
        if (animationGroupListCount > 1)
            yield return new InvalidP3DException(this, $"There can only be one {nameof(AnimationGroupListChunk)} per {nameof(AnimationChunk)}.");
        var animationGroupList = animationGroupListCount == 1 ? GetFirstChunkOfType<AnimationGroupListChunk>() : null;

        if (animationSize != null && animationGroupList != null && animationGroupList.IndexInParent < animationSize.IndexInParent)
            yield return new InvalidP3DException(this, $"The {nameof(AnimationSizeChunk)} must be before the {nameof(AnimationGroupListChunk)}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write((uint)AnimationType);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(cyclic);
    }

    protected override Chunk CloneSelf() => new AnimationChunk(Version, Name, AnimationType, NumFrames, FrameRate, Cyclic);

    // TODO: Caching
    public uint CalculateMemorySize(Platform platform)
    {
        var animationGroupList = GetLastChunkOfType<AnimationGroupListChunk>();
        if (animationGroupList == null)
            return 0u;

        var total = 0u;
        foreach (var animationGroup in animationGroupList.GetChunksOfType<AnimationGroupChunk>())
            total = animationGroup.CalculateMemorySize(platform, total);

        return total;
    }
}
