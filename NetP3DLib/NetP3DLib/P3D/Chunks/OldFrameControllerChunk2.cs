using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldFrameControllerChunk2 : NamedChunk
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
    public string HierarchyName { get; set; }
    public string AnimationName { get; set; }

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

    public OldFrameControllerChunk2(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Type = (Types)br.ReadUInt32();
        HierarchyName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public OldFrameControllerChunk2(string name, uint version, Types type, string hierarchyName, string animationName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Type = type;
        HierarchyName = hierarchyName;
        AnimationName = animationName;
    }

    public override void Validate()
    {
        if (!HierarchyName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(HierarchyName), HierarchyName);

        if (!AnimationName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(AnimationName), AnimationName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write((uint)Type);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }

    internal override Chunk CloneSelf() => new OldFrameControllerChunk2(Name, Version, Type, HierarchyName, AnimationName);
}