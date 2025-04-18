using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BooleanChannelChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Boolean_Channel;
    
    public uint Version { get; set; }
    public ushort StartState { get; set; }
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
        }
    }
    public List<ushort> Values { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes(StartState));
            data.AddRange(BitConverter.GetBytes(NumValues));
            foreach (var value in Values)
                data.AddRange(BitConverter.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(ushort) + sizeof(uint) + sizeof(ushort) * NumValues;

    public BooleanChannelChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        StartState = br.ReadUInt16();
        var numValues = br.ReadInt32();
        Values.Capacity = numValues;
        for (int i = 0; i < numValues; i++)
            Values.Add(br.ReadUInt16());
    }

    public BooleanChannelChunk(uint version, string param, ushort startState, IList<ushort> values) : base(ChunkID)
    {
        Version = version;
        Param = param;
        StartState = startState;
        Values.AddRange(values);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write(StartState);
        bw.Write(NumValues);
        foreach (var value in Values)
            bw.Write(value);
    }
}