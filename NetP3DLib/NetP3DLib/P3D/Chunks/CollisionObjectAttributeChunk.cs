using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionObjectAttributeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Object_Attribute;

    private ushort isStatic;
    public bool IsStatic
    {
        get => isStatic != 0;
        set => isStatic = (ushort)(value ? 1 : 0);
    }
    public uint DefaultArea { get; set; }
    private ushort canRoll;
    public bool CanRoll
    {
        get => canRoll != 0;
        set => canRoll = (ushort)(value ? 1 : 0);
    }
    private ushort canSlide;
    public bool CanSlide
    {
        get => canSlide != 0;
        set => canSlide = (ushort)(value ? 1 : 0);
    }
    private ushort canSpin;
    public bool CanSpin
    {
        get => canSpin != 0;
        set => canSpin = (ushort)(value ? 1 : 0);
    }
    private ushort canBounce;
    public bool CanBounce
    {
        get => canBounce != 0;
        set => canBounce = (ushort)(value ? 1 : 0);
    }
    public uint ExtraAttribute1 { get; set; }
    public uint ExtraAttribute2 { get; set; }
    public uint ExtraAttribute3 { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(isStatic));
            data.AddRange(BitConverter.GetBytes(DefaultArea));
            data.AddRange(BitConverter.GetBytes(canRoll));
            data.AddRange(BitConverter.GetBytes(canSlide));
            data.AddRange(BitConverter.GetBytes(canSpin));
            data.AddRange(BitConverter.GetBytes(canBounce));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute1));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute2));
            data.AddRange(BitConverter.GetBytes(ExtraAttribute3));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(ushort) + sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public CollisionObjectAttributeChunk(BinaryReader br) : base(ChunkID)
    {
        isStatic = br.ReadUInt16();
        DefaultArea = br.ReadUInt32();
        canRoll = br.ReadUInt16();
        canSlide = br.ReadUInt16();
        canSpin = br.ReadUInt16();
        canBounce = br.ReadUInt16();
        ExtraAttribute1 = br.ReadUInt32();
        ExtraAttribute2 = br.ReadUInt32();
        ExtraAttribute3 = br.ReadUInt32();
    }

    public CollisionObjectAttributeChunk(bool isStatic, uint defaultArea, bool canRoll, bool canSlide, bool canSpin, bool canBounce, uint extraAttribute1, uint extraAttribute2, uint extraAttribute3) : base(ChunkID)
    {
        IsStatic = isStatic;
        DefaultArea = defaultArea;
        CanRoll = canRoll;
        CanSlide = canSlide;
        CanSpin = canSpin;
        CanBounce = canBounce;
        ExtraAttribute1 = extraAttribute1;
        ExtraAttribute2 = extraAttribute2;
        ExtraAttribute3 = extraAttribute3;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(isStatic);
        bw.Write(DefaultArea);
        bw.Write(canRoll);
        bw.Write(canSlide);
        bw.Write(canSpin);
        bw.Write(canBounce);
        bw.Write(ExtraAttribute1);
        bw.Write(ExtraAttribute2);
        bw.Write(ExtraAttribute3);
    }

    internal override Chunk CloneSelf() => new CollisionObjectAttributeChunk(IsStatic, DefaultArea, CanRoll, CanSlide, CanSpin, CanBounce, ExtraAttribute1, ExtraAttribute2, ExtraAttribute3);
}