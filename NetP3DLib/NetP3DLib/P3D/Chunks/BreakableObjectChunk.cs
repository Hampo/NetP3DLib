using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BreakableObjectChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Breakable_Object;
    
    public enum Indexes : int
    {
        Null = -1,
        HydrantBreaking = 3,
        MailboxBreaking = 5,
        ParkingMeterBreaking = 6,
        WoodenCratesBreaking = 7,
        TommacoPlantsBreaking = 8,
        PowerCouplingBreaking = 9,
        PineTreeBreaking = 14,
        OakTreeBreaking = 15,
        BigBarrierBreaking = 16,
        RailCrossBreaking = 17,
        SpaceNeedleBreaking = 18,
        KrustyGlassBreaking = 19,
        CypressTreeBreaking = 20,
        DeadTreeBreaking = 21,
        SkeletonBreaking = 22,
        Willow = 23,
        CarExplosion = 24,
        GlobeLight = 25,
        TreeMorn = 26,
        PalmTreeSmall = 27,
        PalmTreeLarge = 28,
        Stopsign = 29,
        Pumpkin = 30,
        PumpkinMed = 31,
        PumpkinSmall = 32,
        CasinoJump = 33,
    }

    public Indexes Index { get; set; }
    public uint MaxInstances { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes((int)Index));
            data.AddRange(BitConverter.GetBytes(MaxInstances));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(int) + sizeof(uint);

    public BreakableObjectChunk(BinaryReader br) : base(ChunkID)
    {
        Index = (Indexes)br.ReadInt32();
        MaxInstances = br.ReadUInt32();
    }

    public BreakableObjectChunk(Indexes index, uint maxInstances) : base(ChunkID)
    {
        Index = index;
        MaxInstances = maxInstances;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((int)Index);
        bw.Write(MaxInstances);
    }

    internal override Chunk CloneSelf() => new BreakableObjectChunk(Index, MaxInstances);
}