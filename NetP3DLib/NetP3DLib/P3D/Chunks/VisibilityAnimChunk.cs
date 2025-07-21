using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VisibilityAnimChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Visibility_Anim;
    
    public string SceneName { get; set; }
    public uint Version { get; set; }
    public uint NumFrames { get; set; }
    public float FrameRate { get; set; }
    public uint NumChannels => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Visibility_Anim_Channel).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SceneName));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(NumChannels));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(SceneName) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public VisibilityAnimChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        SceneName = br.ReadP3DString();
        Version = br.ReadUInt32();
        NumFrames = br.ReadUInt32();
        FrameRate = br.ReadSingle();
        var numChannels = br.ReadUInt32();
    }

    public VisibilityAnimChunk(string name, string sceneName, uint version, uint numFrames, float frameRate) : base(ChunkID)
    {
        Name = name;
        SceneName = sceneName;
        Version = version;
        NumFrames = numFrames;
        FrameRate = frameRate;
    }

    public override void Validate()
    {
        if (!SceneName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(SceneName), SceneName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SceneName);
        bw.Write(Version);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(NumChannels);
    }
}