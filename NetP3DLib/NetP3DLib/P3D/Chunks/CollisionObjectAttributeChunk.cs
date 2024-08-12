using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Object_Attribute)]
public class CollisionObjectAttributeChunk : Chunk
{
    public ushort StaticAttribute { get; set; }
    public uint DefaultArea { get; set; }
    public ushort CanRoll { get; set; }
    public ushort CanSlide { get; set; }
    public ushort CanSpin { get; set; }
    public ushort CanBounce { get; set; }
    public uint ExtraAttribute1 { get; set; }
    public uint ExtraAttribute2 { get; set; }
    public uint ExtraAttribute3 { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(StaticAttribute));
            data.AddRange(BitConverter.GetBytes(DefaultArea));
            data.AddRange(BitConverter.GetBytes(CanRoll));
            data.AddRange(BitConverter.GetBytes(CanSlide));
            data.AddRange(BitConverter.GetBytes(CanSpin));
            data.AddRange(BitConverter.GetBytes(CanBounce));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute1));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute2));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute3));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(ushort) + sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public CollisionObjectAttributeChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Object_Attribute)
    {
        StaticAttribute = br.ReadUInt16();
        DefaultArea = br.ReadUInt32();
        CanRoll = br.ReadUInt16();
        CanSlide = br.ReadUInt16();
        CanSpin = br.ReadUInt16();
        CanBounce = br.ReadUInt16();
        ExtraAttribute1 = br.ReadUInt32();
        ExtraAttribute2 = br.ReadUInt32();
        ExtraAttribute3 = br.ReadUInt32();
    }

    public CollisionObjectAttributeChunk(ushort staticAttribute, uint defaultArea, ushort canRoll, ushort canSlide, ushort canSpin, ushort canBounce, uint extraAttribute1, uint extraAttribute2, uint extraAttribute3) : base((uint)ChunkIdentifier.Collision_Object_Attribute)
    {
        StaticAttribute = staticAttribute;
        DefaultArea = defaultArea;
        CanRoll = canRoll;
        CanSlide = canSlide;
        CanSpin = canSpin;
        CanBounce = canBounce;
        ExtraAttribute1 = extraAttribute1;
        ExtraAttribute2 = extraAttribute2;
        ExtraAttribute3 = extraAttribute3;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(StaticAttribute);
        bw.Write(DefaultArea);
        bw.Write(CanRoll);
        bw.Write(CanSlide);
        bw.Write(CanSpin);
        bw.Write(CanBounce);
        bw.Write(ExtraAttribute1);
        bw.Write(ExtraAttribute2);
        bw.Write(ExtraAttribute3);
    }
}