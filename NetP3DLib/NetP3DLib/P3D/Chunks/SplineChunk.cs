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
public class SplineChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Spline;
    
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) * 3 * NumPositions;

    public SplineChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numPositions = br.ReadInt32();
        Positions = new(numPositions);
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
    }

    public SplineChunk(string name, IList<Vector3> positions) : base(ChunkID)
    {
        Name = name;
        Positions.AddRange(positions);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (Children.Count != 1 || Children[0].ID != (uint)ChunkIdentifier.Rail_Cam)
            yield return new InvalidP3DException(this, $"Children must be one Rail Cam child chunk.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumPositions);
        foreach (var pos in Positions)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new SplineChunk(Name, Positions);
}