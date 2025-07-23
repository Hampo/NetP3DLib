using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldFrameControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Frame_Controller;
    
    public uint Version { get; set; }
    public string Type { get; set; }
    public float FrameOffset { get; set; }
    public string HierarchyName { get; set; }
    public string AnimationName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Type));
            data.AddRange(BitConverter.GetBytes(FrameOffset));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(HierarchyName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(AnimationName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + sizeof(float) + BinaryExtensions.GetP3DStringLength(HierarchyName) + BinaryExtensions.GetP3DStringLength(AnimationName);

    public OldFrameControllerChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadFourCC();
        FrameOffset = br.ReadSingle();
        HierarchyName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public OldFrameControllerChunk(uint version, string name, string type, float frameOffset, string hierarchyName, string animationName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
        FrameOffset = frameOffset;
        HierarchyName = hierarchyName;
        AnimationName = animationName;
    }

    public override void Validate()
    {
        if (!Type.IsValidFourCC())
            throw new InvalidFourCCException(nameof(Type), Type);

        if (!HierarchyName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(HierarchyName), HierarchyName);

        if (!AnimationName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(AnimationName), AnimationName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(Type);
        bw.Write(FrameOffset);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }

    internal override Chunk CloneSelf() => new OldFrameControllerChunk(Version, Name, Type, FrameOffset, HierarchyName, AnimationName);
}