using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class Vector2DOFChannelChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vector_2D_OF_Channel;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    public Coordinate StaticIndex { get; set; }
    public Vector3 Constants { get; set; }
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
    public List<Vector2> Values { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes((ushort)StaticIndex));
            data.AddRange(BinaryExtensions.GetBytes(Constants));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            foreach (var frame in Frames)
                data.AddRange(BitConverter.GetBytes(frame));
            foreach (var value in Values)
                data.AddRange(BinaryExtensions.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(ushort) + sizeof(float) * 3 + sizeof(uint) + sizeof(ushort) * NumFrames + sizeof(float) * 2 * NumValues;

    public Vector2DOFChannelChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        StaticIndex = (Coordinate)br.ReadUInt16();
        Constants = br.ReadVector3();
        var numFrames = br.ReadInt32();
        Frames = new(numFrames);
        Values = new(numFrames);
        for (var i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (var i = 0; i < numFrames; i++)
            Values.Add(br.ReadVector2());
    }

    public Vector2DOFChannelChunk(uint version, string param, Coordinate staticIndex, Vector3 constants, IList<ushort> frames, IList<Vector2> values) : base(ChunkID)
    {
        Version = version;
        Param = param;
        StaticIndex = staticIndex;
        Constants = constants;
        Frames.AddRange(frames);
        Values.AddRange(values);
    }

    public List<Vector3> GetValues()
    {
        var values = new List<Vector3>(Values.Count);

        Func<Vector2, Vector3> map = StaticIndex switch
        {
            Coordinate.X => v => new Vector3(Constants.X, v.X, v.Y),
            Coordinate.Y => v => new Vector3(v.X, Constants.Y, v.Y),
            Coordinate.Z => v => new Vector3(v.X, v.Y, Constants.Z),
            _ => throw new InvalidDataException($"Invalid {nameof(StaticIndex)} value: {StaticIndex}"),
        };

        foreach (var v in Values)
            values.Add(map(v));

        return values;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (Frames.Count != Values.Count)
            yield return new InvalidP3DException(this, $"{nameof(Frames)} and {nameof(Values)} must have equal counts.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write((ushort)StaticIndex);
        bw.Write(Constants);
        bw.Write(NumFrames);
        foreach (var frame in Frames)
            bw.Write(frame);
        foreach (var value in Values)
            bw.Write(value);
    }

    protected override Chunk CloneSelf() => new Vector2DOFChannelChunk(Version, Param, StaticIndex, Constants, Frames, Values);
}
