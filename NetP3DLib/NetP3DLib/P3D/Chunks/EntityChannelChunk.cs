using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Entity_Channel)]
public class EntityChannelChunk : ParamChunk
{
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
    public List<string> Values { get; } = [];

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
                data.AddRange(BinaryExtensions.GetP3DStringBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(uint) + sizeof(short) * NumValues + (uint)Values.Sum(x => BinaryExtensions.GetP3DStringBytes(x).Length);

    public EntityChannelChunk(BinaryReader br) : base((uint)ChunkIdentifier.Entity_Channel)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        var numFrames = br.ReadInt32();
        Frames.Capacity = numFrames;
        Values.Capacity = numFrames;
        for (var i = 0; i < numFrames; i++)
            Frames.Add(br.ReadUInt16());
        for (var i = 0; i < numFrames; i++)
            Values.Add(br.ReadP3DString());
    }

    public EntityChannelChunk(uint version, string param, IList<ushort> frames, IList<string> values) : base((uint)ChunkIdentifier.Entity_Channel)
    {
        Version = version;
        Param = param;
        Frames.AddRange(frames);
        Values.AddRange(values);
    }

    public override void Validate()
    {
        if (Frames.Count != Values.Count)
            throw new InvalidDataException($"{nameof(Frames)} and {nameof(Values)} must have equal counts.");

        foreach (var value in Values)
        {
            if (value == null)
                throw new InvalidDataException($"No item in {nameof(Values)} can be null.");
            if (Encoding.UTF8.GetBytes(value).Length > 255)
                throw new InvalidDataException($"The max length of any item in {nameof(Values)} is 255 bytes.");
        }

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
            bw.WriteP3DString(value);
    }
}