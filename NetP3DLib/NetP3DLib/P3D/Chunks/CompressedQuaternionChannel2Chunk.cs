using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
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
                short X = (short)Math.Round(value.X * short.MaxValue);
                short Y = (short)Math.Round(value.Y * short.MaxValue);
                short Z = (short)Math.Round(value.Z * short.MaxValue);
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
        Frames = new(numFrames);
        Values = new(numFrames);
        for (uint i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (uint i = 0; i < numFrames; i++)
        {
            var x = br.ReadInt16() / (double)short.MaxValue;
            var y = br.ReadInt16() / (double)short.MaxValue;
            var z = br.ReadInt16() / (double)short.MaxValue;

            var sumOfSquares = x * x + y * y + z * z;
            if (sumOfSquares > 1.0f)
                throw new InvalidP3DException(this, $"Invalid Compressed Quaternion Channel 2.");
            var w = Math.Sqrt(1 - sumOfSquares);

            Values.Add(new((float)x, (float)y, (float)z, (float)w));
        }
    }

    public CompressedQuaternionChannel2Chunk(uint version, string param, List<ushort> frames, List<Quaternion> values) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Frames.AddRange(frames);
        Values.AddRange(values);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (Frames.Count != Values.Count)
            yield return new InvalidP3DException(this, $"The number of ${nameof(Frames)} and ${nameof(Values)} much match.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write(NumFrames);
        foreach (var frame in Frames)
            bw.Write(frame);
        foreach (var value in Values)
        {
            short X = (short)Math.Round(value.X * short.MaxValue);
            short Y = (short)Math.Round(value.Y * short.MaxValue);
            short Z = (short)Math.Round(value.Z * short.MaxValue);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }
    }

    protected override Chunk CloneSelf() => new CompressedQuaternionChannel2Chunk(Version, Param, Frames, Values);
}