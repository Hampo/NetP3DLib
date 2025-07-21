using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrameControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frame_Controller;
    
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
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + 4 + sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(HierarchyName) + BinaryExtensions.GetP3DStringLength(AnimationName);

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
        if (!Type.IsValidFourCC())
            throw new InvalidFourCCException(nameof(Type), Type);

        if (!CycleMode.IsValidFourCC())
            throw new InvalidFourCCException(nameof(CycleMode), CycleMode);

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
        bw.WriteFourCC(CycleMode);
        bw.Write(NumCycles);
        bw.Write(InfiniteCycle);
        bw.WriteP3DString(HierarchyName);
        bw.WriteP3DString(AnimationName);
    }
}