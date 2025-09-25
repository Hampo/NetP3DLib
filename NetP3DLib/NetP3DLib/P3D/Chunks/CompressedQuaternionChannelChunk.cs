using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompressedQuaternionChannelChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Compressed_Quaternion_Channel;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
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
            NumValues = value;
        }
    }
    public List<ushort> Frames { get; } = [];
    public uint NumValues
    {
        get => (uint)Values.Count;
        set
        {
            if (value == NumValues)
                return;

            if (value < NumValues)
            {
                while (NumValues > value)
                    Values.RemoveAt(Values.Count - 1);
            }
            else
            {
                while (NumValues < value)
                    Values.Add(default);
            }
            NumFrames = value;
        }
    }
    public List<Quaternion> Values { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            foreach (var frame in Frames)
                data.AddRange(BitConverter.GetBytes(frame));
            foreach (var value in Values)
            {
                short W = (short)Math.Round(value.W * short.MaxValue);
                short X = (short)Math.Round(value.X * short.MaxValue);
                short Y = (short)Math.Round(value.Y * short.MaxValue);
                short Z = (short)Math.Round(value.Z * short.MaxValue);
                data.AddRange(BitConverter.GetBytes(W));
                data.AddRange(BitConverter.GetBytes(X));
                data.AddRange(BitConverter.GetBytes(Y));
                data.AddRange(BitConverter.GetBytes(Z));
            }

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(uint) + sizeof(ushort) * NumFrames + sizeof(short) * 4 * NumValues;

    public CompressedQuaternionChannelChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        int numFrames = br.ReadInt32();
        Frames = new(numFrames);
        Values = new(numFrames);
        for (uint i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (uint i = 0; i < numFrames; i++)
        {
            short W = br.ReadInt16();
            short X = br.ReadInt16();
            short Y = br.ReadInt16();
            short Z = br.ReadInt16();
            Values.Add(new(X / (float)short.MaxValue, Y / (float)short.MaxValue, Z / (float)short.MaxValue, W / (float)short.MaxValue));
        }
    }

    public CompressedQuaternionChannelChunk(uint version, string param, List<ushort> frames, List<Quaternion> values) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Frames.AddRange(frames);
        Values.AddRange(values);
    }

    public override void Validate()
    {
        if (Frames.Count != Values.Count)
            throw new InvalidDataException($"The number of ${nameof(Frames)} and ${nameof(Values)} much match.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write(NumFrames);
        foreach (var frame in Frames)
            bw.Write(frame);
        foreach (var value in Values)
        {
            short W = (short)Math.Round(value.W * short.MaxValue);
            short X = (short)Math.Round(value.X * short.MaxValue);
            short Y = (short)Math.Round(value.Y * short.MaxValue);
            short Z = (short)Math.Round(value.Z * short.MaxValue);
            bw.Write(W);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }
    }

    internal override Chunk CloneSelf() => new CompressedQuaternionChannelChunk(Version, Param, Frames, Values);
}
