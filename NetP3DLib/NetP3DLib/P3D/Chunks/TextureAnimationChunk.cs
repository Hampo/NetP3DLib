using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureAnimationChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Texture_Animation;
    
    public uint Version { get; set; }
    public string MaterialName { get; set; }
    public uint NumFrames { get; set; }
    public float FrameRate { get; set; }
    public uint Cyclic { get; set; }

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
            data.AddRange(BitConverter.GetBytes(Cyclic));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) + sizeof(uint);

    public TextureAnimationChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        MaterialName = br.ReadP3DString();
        NumFrames = br.ReadUInt32();
        FrameRate = br.ReadSingle();
        Cyclic = br.ReadUInt32();
    }

    public TextureAnimationChunk(string name, uint version, string materialName, uint numFrames, float frameRate, uint cyclic) : base(ChunkID)
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
        if (MaterialName == null)
            throw new InvalidDataException($"{nameof(MaterialName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(MaterialName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(MaterialName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(MaterialName);
        bw.Write(NumFrames);
        bw.Write(FrameRate);
        bw.Write(Cyclic);
    }
}