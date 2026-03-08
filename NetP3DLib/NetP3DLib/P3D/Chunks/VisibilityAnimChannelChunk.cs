using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VisibilityAnimChannelChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Visibility_Anim_Channel;
    
    public ushort StartState { get; set; }
    public uint NumFrames
    {
        get => (uint)(Frames?.Count ?? 0);
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
    public SizeAwareList<uint> Frames { get; }

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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(ushort) + sizeof(uint) + sizeof(uint) * NumFrames;

    public VisibilityAnimChannelChunk(BinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        StartState = br.ReadUInt16();
        var numFrames = br.ReadInt32();
        var frames = new uint[numFrames];
        for (int i = 0; i < numFrames; i++)
            frames[i] = br.ReadUInt32();
        Frames = CreateSizeAwareList(frames);
    }

    public VisibilityAnimChannelChunk(string name, ushort startState, IList<uint> frames) : base(ChunkID)
    {
        _name = new(this, name);
        StartState = startState;
        Frames = CreateSizeAwareList(frames);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(StartState);
        bw.Write(NumFrames);
        foreach (var value in Frames)
            bw.Write(value);
    }

    protected override Chunk CloneSelf() => new VisibilityAnimChannelChunk(Name, StartState, Frames);
}