using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light;
    
    public enum Types : uint
    {
        Ambient = 0,
        Directional = 2,
    }

    public uint Version { get; set; }
    public Types Type { get; set; }
    public Color Colour { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Squared { get; set; }
    public uint Enabled { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes((uint)Type));
            data.AddRange(BinaryExtensions.GetBytes(Colour));
            data.AddRange(BitConverter.GetBytes(Constant));
            data.AddRange(BitConverter.GetBytes(Linear));
            data.AddRange(BitConverter.GetBytes(Squared));
            data.AddRange(BitConverter.GetBytes(Enabled));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(uint);

    public LightChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Type = (Types)br.ReadUInt32();
        Colour = br.ReadColor();
        Constant = br.ReadSingle();
        Linear = br.ReadSingle();
        Squared = br.ReadSingle();
        Enabled = br.ReadUInt32();
    }

    public LightChunk(string name, uint version, Types type, Color colour, float constant, float linear, float squared, uint enabled) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Type = type;
        Colour = colour;
        Constant = constant;
        Linear = linear;
        Squared = squared;
        Enabled = enabled;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write((uint)Type);
        bw.Write(Colour);
        bw.Write(Constant);
        bw.Write(Linear);
        bw.Write(Squared);
        bw.Write(Enabled);
    }

    internal override Chunk CloneSelf() => new LightChunk(Name, Version, Type, Colour, Constant, Linear, Squared, Enabled);
}