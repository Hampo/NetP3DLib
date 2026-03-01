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
public class FrameControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frame_Controller;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    private string _type = string.Empty;
    [MaxLength(4)]
    public string Type
    {
        get => _type;
        set
        {
            if (_type == value)
                return;

            _type = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _cycleMode = string.Empty;
    [MaxLength(4)]
    public string CycleMode
    {
        get => _cycleMode;
        set
        {
            if (_cycleMode == value)
                return;

            _cycleMode = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public uint NumCycles { get; set; }
    public uint InfiniteCycle { get; set; }
    private string _hierarchyName = string.Empty;
    public string HierarchyName
    {
        get => _hierarchyName;
        set
        {
            if (_hierarchyName == value)
                return;

            _hierarchyName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
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

    protected override void WriteData(BinaryWriter bw)
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
