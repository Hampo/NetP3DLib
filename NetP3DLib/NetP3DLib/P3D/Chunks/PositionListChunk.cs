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
public class PositionListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Position_List;
    
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
            RecalculateSize();
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
        Positions = new(numPositions);
        for (var i = 0; i < numPositions; i++)
            Positions.Add(br.ReadVector3());
    }

    public PositionListChunk(IList<Vector3> positions) : base(ChunkID)
    {
        Positions.AddRange(positions);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumPositions)
            yield return new InvalidP3DException(this, $"Num Positions value {NumPositions} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumPositions);
        foreach (var pos in Positions)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new PositionListChunk(Positions);
}