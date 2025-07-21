using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation;
    
    public uint Version { get; set; }
    public string AnimationType { get; set; }
    public float NumFrames { get; set; }
    public float FrameRate { get; set; }
    public uint Cyclic { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(AnimationType));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(Cyclic));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + sizeof(float) + sizeof(float) + sizeof(uint);

    public AnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        AnimationType = br.ReadFourCC();
        NumFrames = br.ReadSingle();
        FrameRate = br.ReadSingle();
        Cyclic = br.ReadUInt32();
    }

    public AnimationChunk(uint version, string name, string animationType, float numFrames, float frameRate, uint cyclic) : base(ChunkID)
    {
        Version = version;
        Name = name;
        AnimationType = animationType;
        NumFrames = numFrames;
        FrameRate = frameRate;
        Cyclic = cyclic;
    }

    public override void Validate()
    {
        if (!AnimationType.IsValidFourCC())
            throw new InvalidFourCCException(nameof(AnimationType), AnimationType);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(AnimationType);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(Cyclic);
    }
}