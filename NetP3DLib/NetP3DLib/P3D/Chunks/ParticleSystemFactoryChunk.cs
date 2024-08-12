using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Particle_System_Factory)]
public class ParticleSystemFactoryChunk : NamedChunk
{
    public uint Version { get; set; }
    public float FrameRate { get; set; }
    public uint NumAnimFrames { get; set; }
    public uint NumOLFrames { get; set; }
    public ushort CycleAnim { get; set; }
    public ushort EnableSorting { get; set; }
    public uint NumEmitters => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Sprite_Emitter).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(NumAnimFrames));
            data.AddRange(BitConverter.GetBytes(NumOLFrames));
            data.AddRange(BitConverter.GetBytes(CycleAnim));
            data.AddRange(BitConverter.GetBytes(EnableSorting));
            data.AddRange(BitConverter.GetBytes(NumEmitters));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float) + sizeof(uint) + sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public ParticleSystemFactoryChunk(BinaryReader br) : base((uint)ChunkIdentifier.Particle_System_Factory)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FrameRate = br.ReadSingle();
        NumAnimFrames = br.ReadUInt32();
        NumOLFrames = br.ReadUInt32();
        CycleAnim = br.ReadUInt16();
        EnableSorting = br.ReadUInt16();
        var numEmitters = br.ReadUInt32();
    }

    public ParticleSystemFactoryChunk(uint version, string name, float frameRate, uint numAnimFrames, uint numOLFrames, ushort cycleAnim, ushort enableSorting) : base((uint)ChunkIdentifier.Particle_System_Factory)
    {
        Version = version;
        Name = name;
        FrameRate = frameRate;
        NumAnimFrames = numAnimFrames;
        NumOLFrames = numOLFrames;
        CycleAnim = cycleAnim;
        EnableSorting = enableSorting;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(FrameRate);
        bw.Write(NumAnimFrames);
        bw.Write(NumOLFrames);
        bw.Write(CycleAnim);
        bw.Write(EnableSorting);
        bw.Write(NumEmitters);
    }
}