using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class Vector1DOFChannelChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vector_1D_OF_Channel;
    
    public uint Version { get; set; }
    public Coordinate DynamicIndex { get; set; }
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
    public List<float> Values { get; } = [];

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

    public Vector1DOFChannelChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        DynamicIndex = (Coordinate)br.ReadUInt16();
        Constants = br.ReadVector3();
        var numFrames = br.ReadInt32();
        Frames = new(numFrames);
        Values = new(numFrames);
        for (var i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (var i = 0; i < numFrames; i++)
            Values.Add(br.ReadSingle());
    }

    public Vector1DOFChannelChunk(uint version, string param, Coordinate dynamicIndex, Vector3 constants, IList<ushort> frames, IList<float> values) : base(ChunkID)
    {
        Version = version;
        Param = param;
        DynamicIndex = dynamicIndex;
        Constants = constants;
        Frames.AddRange(frames);
        Values.AddRange(values);
    }

    public List<Vector3> GetValues()
    {
        var values = new List<Vector3>(Values.Count);

        Func<float, Vector3> map = DynamicIndex switch
        {
            Coordinate.X => v => new Vector3(v, Constants.Y, Constants.Z),
            Coordinate.Y => v => new Vector3(Constants.X, v, Constants.Z),
            Coordinate.Z => v => new Vector3(Constants.X, Constants.Y, v),
            _ => throw new InvalidDataException($"Invalid {nameof(DynamicIndex)} value: {DynamicIndex}"),
        };

        foreach (var v in Values)
            values.Add(map(v));

        return values;
    }

    public override void Validate()
    {
        if (Frames.Count != Values.Count)
            throw new InvalidDataException($"{nameof(Frames)} and {nameof(Values)} must have equal counts.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
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

    internal override Chunk CloneSelf() => new Vector1DOFChannelChunk(Version, Param, DynamicIndex, Constants, Frames, Values);
}