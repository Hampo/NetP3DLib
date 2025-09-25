using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropFrameControllerDataChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Frame_Controller_Data;
    
    private uint cyclic;
    public bool Cyclic
    {
        get => cyclic != 0;
        set => cyclic = value ? 1u : 0u;
    }
    public uint NumCycles { get; set; }
    private uint holdFrame;
    public bool HoldFrame
    {
        get => holdFrame != 0;
        set => holdFrame = value ? 1u : 0u;
    }
    public float MinFrame { get; set; }
    public float MaxFrame { get; set; }
    public float RelativeSpeed { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(cyclic));
            data.AddRange(BitConverter.GetBytes(NumCycles));
            data.AddRange(BitConverter.GetBytes(holdFrame));
            data.AddRange(BitConverter.GetBytes(MinFrame));
            data.AddRange(BitConverter.GetBytes(MaxFrame));
            data.AddRange(BitConverter.GetBytes(RelativeSpeed));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float);

    public StatePropFrameControllerDataChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        cyclic = br.ReadUInt32();
        NumCycles = br.ReadUInt32();
        holdFrame = br.ReadUInt32();
        MinFrame = br.ReadSingle();
        MaxFrame = br.ReadSingle();
        RelativeSpeed = br.ReadSingle();
    }

    public StatePropFrameControllerDataChunk(string name, bool cyclic, uint numCycles, bool holdFrame, float minFrame, float maxFrame, float relativeSpeed) : base(ChunkID)
    {
        Name = name;
        Cyclic = cyclic;
        NumCycles = numCycles;
        HoldFrame = holdFrame;
        MinFrame = minFrame;
        MaxFrame = maxFrame;
        RelativeSpeed = relativeSpeed;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(cyclic);
        bw.Write(NumCycles);
        bw.Write(holdFrame);
        bw.Write(MinFrame);
        bw.Write(MaxFrame);
        bw.Write(RelativeSpeed);
    }

    internal override Chunk CloneSelf() => new StatePropFrameControllerDataChunk(Name, Cyclic, NumCycles, HoldFrame, MinFrame, MaxFrame, RelativeSpeed);
}