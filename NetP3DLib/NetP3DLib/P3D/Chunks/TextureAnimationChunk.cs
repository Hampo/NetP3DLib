using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureAnimationChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Texture_Animation;
    
    public uint Version { get; set; }
    public string MaterialName { get; set; }
    public uint NumFrames { get; set; }
    public float FrameRate { get; set; }
    private uint cyclic;
    public bool Cyclic
    {
        get => cyclic == 1;
        set => cyclic = value ? 1u : 0u;
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
            data.AddRange(BitConverter.GetBytes(cyclic));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) + sizeof(uint);

    public TextureAnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        MaterialName = br.ReadP3DString();
        NumFrames = br.ReadUInt32();
        FrameRate = br.ReadSingle();
        cyclic = br.ReadUInt32();
    }

    public TextureAnimationChunk(string name, uint version, string materialName, uint numFrames, float frameRate, bool cyclic) : base(ChunkID)
    {
        Name = name;
        Version = version;
        MaterialName = materialName;
        NumFrames = numFrames;
        FrameRate = frameRate;
        Cyclic = cyclic;
    }

    public override void Validate()
    {
        if (!MaterialName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(MaterialName), MaterialName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(MaterialName);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(cyclic);
    }

    internal override Chunk CloneSelf() => new TextureAnimationChunk(Name, Version, MaterialName, NumFrames, FrameRate, Cyclic);
}