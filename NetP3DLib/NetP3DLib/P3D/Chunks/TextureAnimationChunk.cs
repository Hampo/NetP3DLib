using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureAnimationChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Texture_Animation;

    public uint Version { get; set; }
    private readonly P3DString _materialName;
    public string MaterialName
    {
        get => _materialName?.Value ?? string.Empty;
        set => _materialName.Value = value;
    }
    public uint NumFrames { get; set; }
    public float FrameRate { get; set; }
    private uint _cyclic;
    public bool Cyclic
    {
        get => _cyclic == 1;
        set => _cyclic = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(MaterialName));
            data.AddRange(BitConverter.GetBytes(NumFrames));
            data.AddRange(BitConverter.GetBytes(FrameRate));
            data.AddRange(BitConverter.GetBytes(_cyclic));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(MaterialName) + sizeof(uint) + sizeof(float) + sizeof(uint);

    public TextureAnimationChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadP3DString(), br.ReadUInt32(), br.ReadSingle(), br.ReadUInt32())
    {
    }

    public TextureAnimationChunk(string name, uint version, string materialName, uint numFrames, float frameRate, bool cyclic) : this(name, version, materialName, numFrames, frameRate, cyclic ? 1u : 0u)
    {
    }
    

    public TextureAnimationChunk(string name, uint version, string materialName, uint numFrames, float frameRate, uint cyclic) : base(ChunkID, name)
    {
        Version = version;
        _materialName = new(this, materialName);
        NumFrames = numFrames;
        FrameRate = frameRate;
        _cyclic = cyclic;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!MaterialName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(MaterialName), MaterialName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(MaterialName);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(_cyclic);
    }

    protected override Chunk CloneSelf() => new TextureAnimationChunk(Name, Version, MaterialName, NumFrames, FrameRate, Cyclic);
}