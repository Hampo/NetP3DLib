using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Weight_List)]
public class WeightListChunk : Chunk
{
    public uint NumWeights
    {
        get => (uint)Weights.Count;
        set
        {
            if (value == NumWeights)
                return;

            if (value < NumWeights)
            {
                while (NumWeights > value)
                    Weights.RemoveAt(Weights.Count - 1);
            }
            else
            {
                while (NumWeights < value)
                    Weights.Add(default);
            }
        }
    }
    public List<Vector3> Weights { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumWeights));
            foreach (var weight in Weights)
                data.AddRange(BinaryExtensions.GetBytes(weight));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumWeights;

    public WeightListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Weight_List)
    {
        var numWeights = br.ReadInt32();
        Weights.Capacity = numWeights;
        for (var i = 0; i < numWeights; i++)
            Weights.Add(br.ReadVector3());
    }

    public WeightListChunk(IList<Vector3> weights) : base((uint)ChunkIdentifier.Weight_List)
    {
        Weights.AddRange(weights);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumWeights);
        foreach (var weight in Weights)
            bw.Write(weight);
    }
}