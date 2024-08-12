using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Animated_Object_Animation)]
public class AnimatedObjectAnimationChunk : NamedChunk
{
    public uint Version { get; set; }
    public float FrameRate { get; set; }
    public uint NumOldFrameControllers { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(NumOldFrameControllers));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(float) + sizeof(uint);

    public AnimatedObjectAnimationChunk(BinaryReader br) : base((uint)ChunkIdentifier.Animated_Object_Animation)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        FrameRate = br.ReadSingle();
        NumOldFrameControllers = br.ReadUInt32();
    }

    public AnimatedObjectAnimationChunk(uint version, string name, float frameRate, uint numOldFrameControllers) : base((uint)ChunkIdentifier.Animated_Object_Animation)
    {
        Version = version;
        Name = name;
        FrameRate = frameRate;
        NumOldFrameControllers = numOldFrameControllers;
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
        bw.Write(NumOldFrameControllers);
    }
}
