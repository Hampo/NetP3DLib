using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Inst_Particle_System)]
public class InstParticleSystemChunk : Chunk
{
    public enum ParticleTypes
    {
        Null = -1,
        Shrub = 3,
        Garbage = 4,
        OakTreeLeaves = 5,
        Mail = 6,
        PineTreeNeedles = 7,
        Stars = 8,
        SmokeSpray = 9,
        DirtSpray = 10,
        GrassSpray = 11,
        WaterSpray = 12,
        EngineSmokeLight = 13,
        EngineSmokeHeavy = 14,
        EngineSmokeMedium = 16,
        PowerBoxExplosion = 17,
        FrinksCarSpecialEffect = 18,
        FireSpray = 19,
        AlienCameraExplosion = 20,
        HoverBikeFlame = 21,
        CoconutsDroppingShort = 22,
        CoconutsDroppingTall = 23,
        ParkingMeter = 24,
        CarExplosion = 25,
        Popsicles = 26,
    }

    public ParticleTypes ParticleType { get; set; }
    public uint MaxInstances { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes((int)ParticleType));
            data.AddRange(BitConverter.GetBytes(MaxInstances));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(int) + sizeof(uint);

    public InstParticleSystemChunk(BinaryReader br) : base((uint)ChunkIdentifier.Inst_Particle_System)
    {
        ParticleType = (ParticleTypes)br.ReadInt32();
        MaxInstances = br.ReadUInt32();
    }

    public InstParticleSystemChunk(ParticleTypes particleType, uint maxInstances) : base((uint)ChunkIdentifier.Inst_Particle_System)
    {
        ParticleType = particleType;
        MaxInstances = maxInstances;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((int)ParticleType);
        bw.Write(MaxInstances);
    }
}