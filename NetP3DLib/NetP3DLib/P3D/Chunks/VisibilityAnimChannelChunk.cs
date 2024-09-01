using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Visibility_Anim_Channel)]
public class VisibilityAnimChannelChunk : NamedChunk
{
    public ushort StartState { get; set; }
    public uint NumFrames
    {
        get => (uint)Frames.Count;
        set
        {
            if (value == NumFrames)
                return;

            if (value < NumFrames)
            {
                while (NumFrames > value)
                    Frames.RemoveAt(Frames.Count - 1);
            }
            else
            {
                while (NumFrames < value)
                    Frames.Add(default);
            }
        }
    }
    public List<uint> Frames { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(StartState));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            foreach (var value in Frames)
                data.AddRange(BitConverter.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(ushort) + sizeof(uint) + sizeof(uint) * NumFrames;

    public VisibilityAnimChannelChunk(BinaryReader br) : base((uint)ChunkIdentifier.Visibility_Anim_Channel)
    {
        Name = br.ReadP3DString();
        StartState = br.ReadUInt16();
        var numValues = br.ReadInt32();
        Frames.Capacity = numValues;
        for (int i = 0; i < numValues; i++)
            Frames.Add(br.ReadUInt32());
    }

    public VisibilityAnimChannelChunk(string name, ushort startState, IList<uint> values) : base((uint)ChunkIdentifier.Visibility_Anim_Channel)
    {
        Name = name;
        StartState = startState;
        Frames.AddRange(values);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(StartState);
        bw.Write(NumFrames);
        foreach (var value in Frames)
            bw.Write(value);
    }
}