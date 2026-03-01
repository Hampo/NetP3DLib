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
public class WeightListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Weight_List;
    
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
            RecalculateSize();
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

    public WeightListChunk(BinaryReader br) : base(ChunkID)
    {
        var numWeights = br.ReadInt32();
        Weights = new(numWeights);
        for (var i = 0; i < numWeights; i++)
            Weights.Add(br.ReadVector3());
    }

    public WeightListChunk(IList<Vector3> weights) : base(ChunkID)
    {
        Weights.AddRange(weights);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumWeights)
            yield return new InvalidP3DException(this, $"Num Weights value {NumWeights} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumWeights);
        foreach (var weight in Weights)
            bw.Write(weight);
    }

    protected override Chunk CloneSelf() => new WeightListChunk(Weights);
}