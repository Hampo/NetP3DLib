using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + 4 + sizeof(float) + (uint)BinaryExtensions.GetP3DStringBytes(HierarchyName).Length + (uint)BinaryExtensions.GetP3DStringBytes(AnimationName).Length;

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
        if (Type == null || Type.Length == 0)
            throw new InvalidDataException($"{nameof(Type)} must be at least 1 char.");
        if (Type.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(Type)} is 4 chars.");

        if (HierarchyName == null)
            throw new InvalidDataException($"{nameof(HierarchyName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(HierarchyName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(HierarchyName)} is 255 bytes.");

        if (AnimationName == null)
            throw new InvalidDataException($"{nameof(AnimationName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(AnimationName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(AnimationName)} is 255 bytes.");

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
}