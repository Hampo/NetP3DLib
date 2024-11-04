using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropFrameControllerDataChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Frame_Controller_Data;
    
    public uint Cyclic { get; set; }
    public uint NumCycles { get; set; }
    public uint HoldFrame { get; set; }
    public float MinFrame { get; set; }
    public float MaxFrame { get; set; }
    public float RelativeSpeed { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Cyclic));
            data.AddRange(BitConverter.GetBytes(NumCycles));
            data.AddRange(BitConverter.GetBytes(HoldFrame));
            data.AddRange(BitConverter.GetBytes(MinFrame));
            data.AddRange(BitConverter.GetBytes(MaxFrame));
            data.AddRange(BitConverter.GetBytes(RelativeSpeed));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float);

    public StatePropFrameControllerDataChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Cyclic = br.ReadUInt32();
        NumCycles = br.ReadUInt32();
        HoldFrame = br.ReadUInt32();
        MinFrame = br.ReadSingle();
        MaxFrame = br.ReadSingle();
        RelativeSpeed = br.ReadSingle();
    }

    public StatePropFrameControllerDataChunk(string name, uint cyclic, uint numCycles, uint holdFrame, float minFrame, float maxFrame, float relativeSpeed) : base(ChunkID)
    {
        Name = name;
        Cyclic = cyclic;
        NumCycles = numCycles;
        HoldFrame = holdFrame;
        MinFrame = minFrame;
        MaxFrame = maxFrame;
        RelativeSpeed = relativeSpeed;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Cyclic);
        bw.Write(NumCycles);
        bw.Write(HoldFrame);
        bw.Write(MinFrame);
        bw.Write(MaxFrame);
        bw.Write(RelativeSpeed);
    }
}