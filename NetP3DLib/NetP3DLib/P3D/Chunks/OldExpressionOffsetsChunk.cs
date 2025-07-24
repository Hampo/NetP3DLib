using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldExpressionOffsetsChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Expression_Offsets;
    
    public uint NumPrimitiveGroups
    {
        get => (uint)PrimitiveGroupIndices.Count;
        set
        {
            if (value == NumPrimitiveGroups)
                return;

            if (value < NumPrimitiveGroups)
            {
                while (NumPrimitiveGroups > value)
                    PrimitiveGroupIndices.RemoveAt(PrimitiveGroupIndices.Count - 1);
            }
            else
            {
                while (NumPrimitiveGroups < value)
                    PrimitiveGroupIndices.Add(default);
            }
        }
    }
    public uint NumOffsetLists => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Offset_List).Count();
    public List<uint> PrimitiveGroupIndices { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumPrimitiveGroups));
            data.AddRange(BitConverter.GetBytes(NumOffsetLists));
            foreach (var index in PrimitiveGroupIndices)
                data.AddRange(BitConverter.GetBytes(index));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) * NumPrimitiveGroups;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldExpressionOffsetsChunk(BinaryReader br) : base(ChunkID)
    {
        var numPrimitiveGroups = br.ReadInt32();
        var numOffsetLists = br.ReadUInt32();
        PrimitiveGroupIndices = new(numPrimitiveGroups);
        for (var i = 0; i < numPrimitiveGroups; i++)
            PrimitiveGroupIndices.Add(br.ReadUInt32());
    }

    public OldExpressionOffsetsChunk(IList<uint> primitiveGroupIndices) : base(ChunkID)
    {
        PrimitiveGroupIndices.AddRange(primitiveGroupIndices);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumPrimitiveGroups);
        bw.Write(NumOffsetLists);
        foreach (var index in PrimitiveGroupIndices)
            bw.Write(index);
    }

    internal override Chunk CloneSelf() => new OldExpressionOffsetsChunk(PrimitiveGroupIndices);
}