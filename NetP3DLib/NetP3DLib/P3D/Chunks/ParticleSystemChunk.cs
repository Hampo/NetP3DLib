using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Particle_System)]
public class ParticleSystemChunk : NamedChunk
{
    public uint Version { get; set; }
    public float FrameRate { get; set; }
    public uint NumFrames { get; set; }
    public uint IsCyclic { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Translation { get; set; }
    public uint NumEmitters => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Sprite_Particle_Emitter).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(IsCyclic));
            data.AddRange(BinaryExtensions.GetBytes(Rotation));
            data.AddRange(BinaryExtensions.GetBytes(Translation));
            data.AddRange(BitConverter.GetBytes(NumEmitters));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float) + sizeof(uint) + sizeof(uint) + sizeof(float) * 4 + sizeof(float) * 3 + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public ParticleSystemChunk(BinaryReader br) : base((uint)ChunkIdentifier.Particle_System)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FrameRate = br.ReadSingle();
        NumFrames = br.ReadUInt32();
        IsCyclic = br.ReadUInt32();
        Rotation = br.ReadQuaternion();
        Translation = br.ReadVector3();
        var numEmitters = br.ReadUInt32();
    }

    public ParticleSystemChunk(uint version, string name, float frameRate, uint numFrames, uint isCyclic, Quaternion rotation, Vector3 translation) : base((uint)ChunkIdentifier.Particle_System)
    {
        Version = version;
        Name = name;
        FrameRate = frameRate;
        NumFrames = numFrames;
        IsCyclic = isCyclic;
        Rotation = rotation;
        Translation = translation;
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
        bw.Write(NumFrames);
        bw.Write(IsCyclic);
        bw.Write(Rotation);
        bw.Write(Translation);
        bw.Write(NumEmitters);
    }
}