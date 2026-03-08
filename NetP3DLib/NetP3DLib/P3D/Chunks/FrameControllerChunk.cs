using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrameControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frame_Controller;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    private readonly FourCC _type;
    [MaxLength(4)]
    public string Type
    {
        get => _type?.Value ?? string.Empty;
        set => _type.Value = value;
    }
    private readonly FourCC _cycleMode;
    [MaxLength(4)]
    public string CycleMode
    {
        get => _cycleMode?.Value ?? string.Empty;
        set => _cycleMode.Value = value;
    }
    public uint NumCycles { get; set; }
    public uint InfiniteCycle { get; set; }
    private readonly P3DString _hierarchyName;
    public string HierarchyName
    {
        get => _hierarchyName?.Value ?? string.Empty;
        set => _hierarchyName.Value = value;
    }
    private readonly P3DString _animationName;
    public string AnimationName
    {
        get => _animationName?.Value ?? string.Empty;
        set => _animationName.Value = value;
    }

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

    public FrameControllerChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        _type = new(this, br);
        _cycleMode = new(this, br);
        NumCycles = br.ReadUInt32();
        InfiniteCycle = br.ReadUInt32();
        _hierarchyName = new(this, br);
        _animationName = new(this, br);
    }

    public FrameControllerChunk(uint version, string name, string type, string cycleMode, uint numCycles, uint infiniteCycle, string hierarchyName, string animationName) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _type = new(this, type);
        _cycleMode = new(this, cycleMode);
        NumCycles = numCycles;
        InfiniteCycle = infiniteCycle;
        _hierarchyName = new(this, hierarchyName);
        _animationName = new(this, animationName);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Type.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(Type), Type);

        if (!CycleMode.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(CycleMode), CycleMode);

        if (!HierarchyName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(HierarchyName), HierarchyName);

        if (!AnimationName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(AnimationName), AnimationName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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

    protected override Chunk CloneSelf() => new FrameControllerChunk(Version, Name, Type, CycleMode, NumCycles, InfiniteCycle, HierarchyName, AnimationName);
}
