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
public class OldFrameControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Frame_Controller;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public AnimationType Type { get; set; }
    public float FrameOffset { get; set; }
    private string _hierarchyName = string.Empty;
    public string HierarchyName
    {
        get => _hierarchyName;
        set
        {
            if (_hierarchyName == value)
                return;

            _hierarchyName = value;
            RecalculateSize();
        }
    }
    private string _animationName = string.Empty;
    public string AnimationName
    {
        get => _animationName;
        set
        {
            if (_animationName == value)
                return;

            _animationName = value;
            RecalculateSize();
        }
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)Type));
            data.AddRange(BitConverter.GetBytes(FrameOffset));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(HierarchyName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(AnimationName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) + BinaryExtensions.GetP3DStringLength(HierarchyName) + BinaryExtensions.GetP3DStringLength(AnimationName);

    public OldFrameControllerChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = (AnimationType)br.ReadUInt32();
        FrameOffset = br.ReadSingle();
        HierarchyName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public OldFrameControllerChunk(uint version, string name, AnimationType type, float frameOffset, string hierarchyName, string animationName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
        FrameOffset = frameOffset;
        HierarchyName = hierarchyName;
        AnimationName = animationName;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!HierarchyName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(HierarchyName), HierarchyName);

        if (!AnimationName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(AnimationName), AnimationName);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write((uint)Type);
        bw.Write(FrameOffset);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }

    protected override Chunk CloneSelf() => new OldFrameControllerChunk(Version, Name, Type, FrameOffset, HierarchyName, AnimationName);
}
