using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Spline)]
public class SplineChunk : NamedChunk
{
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

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumPositions));
            foreach (var pos in Positions)
                data.AddRange(BinaryExtensions.GetBytes(pos));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) * 3 * NumPositions;

    public SplineChunk(BinaryReader br) : base((uint)ChunkIdentifier.Spline)
    {
        Name = br.ReadP3DString();
        var numPositions = br.ReadInt32();
        Positions.Capacity = numPositions;
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
    }

    public SplineChunk(string name, IList<Vector3> positions) : base((uint)ChunkIdentifier.Spline)
    {
        Name = name;
        Positions.AddRange(positions);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumPositions);
        foreach (var pos in Positions)
            bw.Write(pos);
    }
}