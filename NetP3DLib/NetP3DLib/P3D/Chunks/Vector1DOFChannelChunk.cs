using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class Vector1DOFChannelChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vector_1D_OF_Channel;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public Coordinate DynamicIndex { get; set; }
    public Vector3 Constants { get; set; }
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
    public SizeAwareList<float> Values { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes((ushort)DynamicIndex));
            data.AddRange(BinaryExtensions.GetBytes(Constants));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            foreach (var frame in Frames)
                data.AddRange(BitConverter.GetBytes(frame));
            foreach (var value in Values)
                data.AddRange(BitConverter.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(ushort) + sizeof(float) * 3 + sizeof(uint) + sizeof(ushort) * NumFrames + sizeof(float) * NumValues;

    public Vector1DOFChannelChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadFourCC(), (Coordinate)br.ReadUInt16(), br.ReadVector3(), ListHelper.ReadArray(br.ReadInt32, br.ReadUInt16, out var numFrames), ListHelper.ReadArray(numFrames, br.ReadSingle))
    {
    }

    public Vector1DOFChannelChunk(uint version, string param, Coordinate dynamicIndex, Vector3 constants, IList<ushort> frames, IList<float> values) : base(ChunkID, param)
    {
        Version = version;
        DynamicIndex = dynamicIndex;
        Constants = constants;
        Frames = CreateSizeAwareList(frames);
        Values = CreateSizeAwareList(values);
    }

    public Vector3[] GetValues()
    {
        var values = new Vector3[Values.Count];

        Func<float, Vector3> map = DynamicIndex switch
        {
            Coordinate.X => v => new Vector3(v, Constants.Y, Constants.Z),
            Coordinate.Y => v => new Vector3(Constants.X, v, Constants.Z),
            Coordinate.Z => v => new Vector3(Constants.X, Constants.Y, v),
            _ => throw new InvalidDataException($"Invalid {nameof(DynamicIndex)} value: {DynamicIndex}"),
        };

        for (var i = 0; i < Values.Count; i++)
            values[i] = map(Values[i]);

        return values;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Frames.Count != Values.Count)
            yield return new InvalidP3DException(this, $"{nameof(Frames)} and {nameof(Values)} must have equal counts.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write((ushort)DynamicIndex);
        bw.Write(Constants);
        bw.Write(NumFrames);
        foreach (var frame in Frames)
            bw.Write(frame);
        foreach (var value in Values)
            bw.Write(value);
    }

    protected override Chunk CloneSelf() => new Vector1DOFChannelChunk(Version, Param, DynamicIndex, Constants, Frames, Values);

    internal uint CalculateMemorySize(AnimationChunk.Platform platform, uint size)
    {
        size = ((size + 3u) & ~3u) + (platform == AnimationChunk.Platform.PS2 ? 52u : 48u);
        size = ((size + 3u) & ~3u) + NumFrames * 2;
        size = ((size + 3u) & ~3u) + NumFrames * 4;

        return size;
    }
}
