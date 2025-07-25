using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiController2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Controller_2;
    
    public uint Version { get; set; }
    public string CycleMode { get; set; }
    public uint NumCycles { get; set; }
    public uint InfiniteCycle { get; set; }
    public float NumFrames { get; set; }
    public float FrameRate { get; set; }
    public uint NumTracks => GetChildCount(ChunkIdentifier.Multi_Controller_Track);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(CycleMode));
            data.AddRange(BitConverter.GetBytes(NumCycles));
            data.AddRange(BitConverter.GetBytes(InfiniteCycle));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(NumTracks));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public MultiController2Chunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        CycleMode = br.ReadFourCC();
        NumCycles = br.ReadUInt32();
        InfiniteCycle = br.ReadUInt32();
        NumFrames = br.ReadSingle();
        FrameRate = br.ReadSingle();
        var numTracks = br.ReadUInt32();
    }

    public MultiController2Chunk(uint version, string name, string cycleMode, uint numCycles, uint infiniteCycle, float numFrames, float frameRate) : base(ChunkID)
    {
        Version = version;
        Name = name;
        CycleMode = cycleMode;
        NumCycles = numCycles;
        InfiniteCycle = infiniteCycle;
        NumFrames = numFrames;
        FrameRate = frameRate;
    }

    public override void Validate()
    {
        if (!CycleMode.IsValidFourCC())
            throw new InvalidFourCCException(nameof(CycleMode), CycleMode);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(CycleMode);
        bw.Write(NumCycles);
        bw.Write(InfiniteCycle);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(NumTracks);
    }

    internal override Chunk CloneSelf() => new MultiController2Chunk(Version, Name, CycleMode, NumCycles, InfiniteCycle, NumFrames, FrameRate);
}