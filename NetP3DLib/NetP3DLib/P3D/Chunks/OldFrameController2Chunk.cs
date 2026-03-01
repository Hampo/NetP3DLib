using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldFrameController2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Frame_Controller_2;
    
    public enum Types
    {
        Undefined,
        Camera,
        Expression,
        Light,
        PolySkin,
        CompoundMesh,
        ScenegraphVisibility,
        DeformPolySkin,
        Texture,
        ScenegraphTransform,
        HSplineOffsetAbsolute,
        HSplineOffsetRelative,
        HSplineSkin,
        Effect,
        CompositeDrawable,
        CompositeDrawableVisibility,
    }

    public uint Version { get; set; }
    public Types Type { get; set; }
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

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes((uint)Type));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(HierarchyName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(AnimationName));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(HierarchyName) + BinaryExtensions.GetP3DStringLength(AnimationName);

    public OldFrameController2Chunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Type = (Types)br.ReadUInt32();
        HierarchyName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public OldFrameController2Chunk(string name, uint version, Types type, string hierarchyName, string animationName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Type = type;
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
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write((uint)Type);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }

    protected override Chunk CloneSelf() => new OldFrameController2Chunk(Name, Version, Type, HierarchyName, AnimationName);
}