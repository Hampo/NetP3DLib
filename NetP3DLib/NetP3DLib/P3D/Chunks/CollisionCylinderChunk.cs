using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionCylinderChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Cylinder;

    public float Radius { get; set; }
    public float HalfLength { get; set; }
    private ushort flatEnd;
    public bool FlatEnd
    {
        get => flatEnd != 0;
        set => flatEnd = (ushort)(value ? 1 : 0);
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Radius));
            data.AddRange(BitConverter.GetBytes(HalfLength));
            data.AddRange(BitConverter.GetBytes(flatEnd));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) + sizeof(float) + sizeof(ushort);

    public CollisionCylinderChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Radius = br.ReadSingle();
        HalfLength = br.ReadSingle();
        flatEnd = br.ReadUInt16();
    }

    public CollisionCylinderChunk(float radius, float halfLength, bool flatEnd) : base(ChunkID)
    {
        Radius = radius;
        HalfLength = halfLength;
        FlatEnd = flatEnd;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Children.Count < 2)
            yield return new InvalidP3DException(this, $"First two children must be {nameof(CollisionVectorChunk)}s.");
        else
        {
            if (Children[0].ID != (uint)ChunkIdentifier.Collision_Vector)
                yield return new InvalidP3DException(this, $"First child is not {nameof(CollisionVectorChunk)}.");

            if (Children[1].ID != (uint)ChunkIdentifier.Collision_Vector)
                yield return new InvalidP3DException(this, $"Second child is not {nameof(CollisionVectorChunk)}.");
        }
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Radius);
        bw.Write(HalfLength);
        bw.Write(flatEnd);
    }

    protected override Chunk CloneSelf() => new CollisionCylinderChunk(Radius, HalfLength, FlatEnd);
}