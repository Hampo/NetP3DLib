using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Cylinder)]
public class CollisionCylinderChunk : Chunk
{
    public float Radius { get; set; }
    public float HalfLength { get; set; }
    public ushort FlatEnd { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Radius));
            data.AddRange(BitConverter.GetBytes(HalfLength));
            data.AddRange(BitConverter.GetBytes(FlatEnd));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) + sizeof(float) + sizeof(ushort);

    public CollisionCylinderChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Cylinder)
    {
        Radius = br.ReadSingle();
        HalfLength = br.ReadSingle();
        FlatEnd = br.ReadUInt16();
    }

    public CollisionCylinderChunk(float cylinderRadius, float length, ushort flatEnd) : base((uint)ChunkIdentifier.Collision_Cylinder)
    {
        Radius = cylinderRadius;
        HalfLength = length;
        FlatEnd = flatEnd;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Radius);
        bw.Write(HalfLength);
        bw.Write(FlatEnd);
    }
}