using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPure3DObjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Pure3D_Object;
    
    public enum Justifications : uint
    {
        Left,
        Right,
        Top,
        Bottom,
        Centre
    }

    public uint Version { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public uint DimensionX { get; set; }
    public uint DimensionY { get; set; }
    public Justifications JustificationX { get; set; }
    public Justifications JustificationY { get; set; }
    public Color Colour { get; set; }
    public uint Translucency { get; set; }
    public float RotationValue { get; set; }
    public string Pure3DFilename { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(PositionX));
            data.AddRange(BitConverter.GetBytes(PositionY));
            data.AddRange(BitConverter.GetBytes(DimensionX));
            data.AddRange(BitConverter.GetBytes(DimensionY));
            data.AddRange(BitConverter.GetBytes((uint)JustificationX));
            data.AddRange(BitConverter.GetBytes((uint)JustificationY));
            data.AddRange(BinaryExtensions.GetBytes(Colour));
            data.AddRange(BitConverter.GetBytes(Translucency));
            data.AddRange(BitConverter.GetBytes(RotationValue));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Pure3DFilename));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + (uint)BinaryExtensions.GetP3DStringBytes(Pure3DFilename).Length;

    public FrontendPure3DObjectChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        PositionX = br.ReadInt32();
        PositionY = br.ReadInt32();
        DimensionX = br.ReadUInt32();
        DimensionY = br.ReadUInt32();
        JustificationX = (Justifications)br.ReadUInt32();
        JustificationY = (Justifications)br.ReadUInt32();
        Colour = br.ReadColor();
        Translucency = br.ReadUInt32();
        RotationValue = br.ReadSingle();
        Pure3DFilename = br.ReadP3DString();
    }

    public FrontendPure3DObjectChunk(string name, uint version, int positionX, int positionY, uint dimensionX, uint dimensionY, Justifications justificationX, Justifications justificationY, Color colour, uint translucency, float rotationValue, string pure3DFilename) : base(ChunkID)
    {
        Name = name;
        Version = version;
        PositionX = positionX;
        PositionY = positionY;
        DimensionX = dimensionX;
        DimensionY = dimensionY;
        JustificationX = justificationX;
        JustificationY = justificationY;
        Colour = colour;
        Translucency = translucency;
        RotationValue = rotationValue;
        Pure3DFilename = pure3DFilename;
    }

    public override void Validate()
    {
        if (Pure3DFilename == null)
            throw new InvalidDataException($"{nameof(Pure3DFilename)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Pure3DFilename).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Pure3DFilename)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(PositionX);
        bw.Write(PositionY);
        bw.Write(DimensionX);
        bw.Write(DimensionY);
        bw.Write((uint)JustificationX);
        bw.Write((uint)JustificationY);
        bw.Write(Colour);
        bw.Write(Translucency);
        bw.Write(RotationValue);
        bw.WriteP3DString(Pure3DFilename);
    }
}