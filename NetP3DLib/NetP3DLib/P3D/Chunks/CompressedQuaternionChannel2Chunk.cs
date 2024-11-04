using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompressedQuaternionChannel2Chunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Compressed_Quaternion_Channel_2;
    
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
                short X = (short)Math.Round(value.X * 32767);
                short Y = (short)Math.Round(value.Y * 32767);
                short Z = (short)Math.Round(value.Z * 32767);
                data.AddRange(BitConverter.GetBytes(X));
                data.AddRange(BitConverter.GetBytes(Y));
                data.AddRange(BitConverter.GetBytes(Z));
            }

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(uint) + sizeof(ushort) * NumFrames + sizeof(short) * 3 * NumValues;

    public CompressedQuaternionChannel2Chunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        int numFrames = br.ReadInt32();
        Frames.Capacity = numFrames;
        Values.Capacity = numFrames;
        for (uint i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (uint i = 0; i < numFrames; i++)
        {
            float X = br.ReadInt16() / 32767f;
            float Y = br.ReadInt16() / 32767f;
            float Z = br.ReadInt16() / 32767f;
            float W = (float)Math.Sqrt(1 - Math.Pow(X, 2) - Math.Pow(Y, 2) - Math.Pow(Z, 2));
            Values.Add(new(X, Y, Z, W));
        }
    }

    public CompressedQuaternionChannel2Chunk(uint version, string param, List<ushort> frames, List<Quaternion> values) : base(ChunkID)
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
            short X = (short)Math.Round(value.X * 32767);
            short Y = (short)Math.Round(value.Y * 32767);
            short Z = (short)Math.Round(value.Z * 32767);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }
    }
}