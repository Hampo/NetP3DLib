using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PositionListChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Position_List;
    
    public uint NumPositions
    {
        get => (uint)Positions.Count;
        set
        {
            if (value == NumPositions)
                return;

            if (value < NumPositions)
            {
                while (NumPositions > value)
                    Positions.RemoveAt(Positions.Count - 1);
            }
            else
            {
                while (NumPositions < value)
                    Positions.Add(default);
            }
        }
    }
    public List<Vector3> Positions { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumPositions));
            foreach (var pos in Positions)
                data.AddRange(BinaryExtensions.GetBytes(pos));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumPositions;

    public PositionListChunk(BinaryReader br) : base(ChunkID)
    {
        var numPositions = br.ReadInt32();
        Positions.Capacity = numPositions;
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
    }

    public PositionListChunk(IList<Vector3> positions) : base(ChunkID)
    {
        Positions.AddRange(positions);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumPositions);
        foreach (var pos in Positions)
            bw.Write(pos);
    }
}