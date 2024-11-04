using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrameControllerChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frame_Controller;
    
    public uint Version { get; set; }
    public string Type { get; set; }
    public string CycleMode { get; set; }
    public uint NumCycles { get; set; }
    public uint InfiniteCycle { get; set; }
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
            data.AddRange(BinaryExtensions.GetFourCCBytes(CycleMode));
            data.AddRange(BitConverter.GetBytes(NumCycles));
            data.AddRange(BitConverter.GetBytes(InfiniteCycle));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(HierarchyName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(AnimationName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + 4 + 4 + sizeof(uint) + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(HierarchyName).Length + (uint)BinaryExtensions.GetP3DStringBytes(AnimationName).Length;

    public FrameControllerChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadFourCC();
        CycleMode = br.ReadFourCC();
        NumCycles = br.ReadUInt32();
        InfiniteCycle = br.ReadUInt32();
        HierarchyName = br.ReadP3DString();
        AnimationName = br.ReadP3DString();
    }

    public FrameControllerChunk(uint version, string name, string type, string cycleMode, uint numCycles, uint infiniteCycle, string hierarchyName, string animationName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
        CycleMode = cycleMode;
        NumCycles = numCycles;
        InfiniteCycle = infiniteCycle;
        HierarchyName = hierarchyName;
        AnimationName = animationName;
    }

    public override void Validate()
    {
        if (Type == null || Type.Length == 0)
            throw new InvalidDataException($"{nameof(Type)} must be at least 1 char.");
        if (Type.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(Type)} is 4 chars.");

        if (CycleMode == null || CycleMode.Length == 0)
            throw new InvalidDataException($"{nameof(CycleMode)} must be at least 1 char.");
        if (CycleMode.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(CycleMode)} is 4 chars.");

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
        bw.WriteFourCC(CycleMode);
        bw.Write(NumCycles);
        bw.Write(InfiniteCycle);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }
}