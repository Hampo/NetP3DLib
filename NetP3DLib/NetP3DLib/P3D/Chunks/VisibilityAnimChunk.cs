using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VisibilityAnimChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Visibility_Anim;

    private readonly P3DString _sceneName;
    public string SceneName
    {
        get => _sceneName?.Value ?? string.Empty;
        set => _sceneName.Value = value;
    }
    public uint Version { get; set; }
    public uint NumFrames { get; set; }
    public float FrameRate { get; set; }
    public uint NumChannels => GetChildCount(ChunkIdentifier.Visibility_Anim_Channel);

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
    public VisibilityAnimChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        _sceneName = new(this, br);
        Version = br.ReadUInt32();
        NumFrames = br.ReadUInt32();
        FrameRate = br.ReadSingle();
        var numChannels = br.ReadUInt32();
    }

    public VisibilityAnimChunk(string name, string sceneName, uint version, uint numFrames, float frameRate) : base(ChunkID)
    {
        _name = new(this, name);
        _sceneName = new(this, sceneName);
        Version = version;
        NumFrames = numFrames;
        FrameRate = frameRate;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!SceneName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(SceneName), SceneName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SceneName);
        bw.Write(Version);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(NumChannels);
    }

    protected override Chunk CloneSelf() => new VisibilityAnimChunk(Name, SceneName, Version, NumFrames, FrameRate);
}