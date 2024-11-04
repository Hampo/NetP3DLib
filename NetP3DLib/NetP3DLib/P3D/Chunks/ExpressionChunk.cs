using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Expression;
    
    public uint Version { get; set; }
    public uint NumKeys
    {
        get => (uint)Keys.Count;
        set
        {
            if (value == NumKeys)
                return;

            if (value < NumKeys)
            {
                while (NumKeys > value)
                    Keys.RemoveAt(Keys.Count - 1);
            }
            else
            {
                while (NumKeys < value)
                    Keys.Add(default);
            }
            NumIndices = value;
        }
    }
    public List<float> Keys { get; } = [];
    public uint NumIndices
    {
        get => (uint)Indices.Count;
        set
        {
            if (value == NumIndices)
                return;

            if (value < NumIndices)
            {
                while (NumIndices > value)
                    Indices.RemoveAt(Indices.Count - 1);
            }
            else
            {
                while (NumIndices < value)
                    Indices.Add(default);
            }
            NumKeys = value;
        }
    }
    public List<uint> Indices { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumKeys));
            foreach (var key in Keys)
                data.AddRange(BitConverter.GetBytes(key));
            foreach (var index in Indices)
                data.AddRange(BitConverter.GetBytes(index));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) * NumKeys + sizeof(uint) * NumIndices;

    public ExpressionChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        var numKeys = br.ReadInt32();
        Keys.Capacity = numKeys;
        Indices.Capacity = numKeys;
        for (int i = 0; i < numKeys; i++)
            Keys.Add(br.ReadSingle());
        for (int i = 0; i < numKeys; i++)
            Indices.Add(br.ReadUInt32());
    }

    public ExpressionChunk(uint version, string name, IList<float> keys, IList<uint> indices) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Keys.AddRange(keys);
        Indices.AddRange(indices);
    }

    public override void Validate()
    {
        if (Keys.Count != Indices.Count)
            throw new InvalidDataException($"{nameof(Keys)} and {nameof(Indices)} must have equal counts.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(NumKeys);
        foreach (var key in Keys)
            bw.Write(key);
        foreach (var index in Indices)
            bw.Write(index);
    }
}