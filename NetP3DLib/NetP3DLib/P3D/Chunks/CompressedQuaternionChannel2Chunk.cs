using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompressedQuaternionChannel2Chunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Compressed_Quaternion_Channel_2;

    private uint _version;
    public uint Version
    {
        get => _version;
        set
        {
            if (_version == value)
                return;
    
            _version = value;
            OnPropertyChanged(nameof(Version));
        }
    }
    
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
            NumValues = value;
        }
    }
    public SizeAwareList<ushort> Frames { get; }
    public uint NumValues
    {
        get => (uint)(Values?.Count ?? 0);
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
    public SizeAwareList<Quaternion> Values { get; }

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

    public CompressedQuaternionChannel2Chunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadFourCC(), ListHelper.ReadArray(br.ReadInt32, br.ReadUInt16, out var numFrames), ListHelper.ReadArray(numFrames, () =>
        {
            var x = br.ReadInt16() / (double)short.MaxValue;
            var y = br.ReadInt16() / (double)short.MaxValue;
            var z = br.ReadInt16() / (double)short.MaxValue;

            var sumOfSquares = x * x + y * y + z * z;
            if (sumOfSquares > 1.0f)
                throw new InvalidDataException("Invalid Compressed Quaternion Channel 2.");
            var w = Math.Sqrt(1 - sumOfSquares);

            return new Quaternion((float)x, (float)y, (float)z, (float)w);
        }))
    {
    }

    public CompressedQuaternionChannel2Chunk(uint version, string param, IList<ushort> frames, IList<Quaternion> values) : base(ChunkID, param)
    {
        _version = version;
        Frames = CreateSizeAwareList(frames, Frames_CollectionChanged);
        Values = CreateSizeAwareList(values, Values_CollectionChanged);
    }
    
    private void Frames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(Frames));
    
    private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(Values));

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Frames.Count != Values.Count)
            yield return new InvalidP3DException(this, $"The number of ${nameof(Frames)} and ${nameof(Values)} much match.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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
