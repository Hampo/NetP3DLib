using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

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

    [DefaultValue(257)]
    public uint Version { get; set; }
    public Types Type { get; set; }
    public Color Colour { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Squared { get; set; }
    private uint _enabled;
    public bool Enabled
    {
        get => _enabled != 0;
        set => _enabled = value ? 1u : 0u;
    }

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
            data.AddRange(BitConverter.GetBytes(_enabled));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(uint);

    public LightChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), (Types)br.ReadUInt32(), br.ReadColor(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadUInt32())
    {
    }

    public LightChunk(string name, uint version, Types type, Color colour, float constant, float linear, float squared, bool enabled) : this(name, version, type, colour, constant, linear, squared, enabled ? 1u : 0u)
    {
    }

    public LightChunk(string name, uint version, Types type, Color colour, float constant, float linear, float squared, uint enabled) : base(ChunkID, name)
    {
        Version = version;
        Type = type;
        Colour = colour;
        Constant = constant;
        Linear = linear;
        Squared = squared;
        _enabled = enabled;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write((uint)Type);
        bw.Write(Colour);
        bw.Write(Constant);
        bw.Write(Linear);
        bw.Write(Squared);
        bw.Write(_enabled);
    }

    protected override Chunk CloneSelf() => new LightChunk(Name, Version, Type, Colour, Constant, Linear, Squared, Enabled);
}
