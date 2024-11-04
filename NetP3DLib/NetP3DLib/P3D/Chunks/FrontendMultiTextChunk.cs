using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendMultiTextChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_Multi_Text;
    
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
    public string TextStyleName { get; set; }
    public byte ShadowEnabled { get; set; }
    public Color ShadowColour { get; set; }
    public int ShadowOffsetX { get; set; }
    public int ShadowOffsetY { get; set; }
    public uint CurrentText { get; set; }

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
            data.AddRange(BinaryExtensions.GetP3DStringBytes(TextStyleName));
            data.Add(ShadowEnabled);
            data.AddRange(BinaryExtensions.GetBytes(ShadowColour));
            data.AddRange(BitConverter.GetBytes(ShadowOffsetX));
            data.AddRange(BitConverter.GetBytes(ShadowOffsetY));
            data.AddRange(BitConverter.GetBytes(CurrentText));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + (uint)BinaryExtensions.GetP3DStringBytes(TextStyleName).Length + sizeof(byte) + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint);

    public FrontendMultiTextChunk(BinaryReader br) : base(ChunkID)
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
        TextStyleName = br.ReadP3DString();
        ShadowEnabled = br.ReadByte();
        ShadowColour = br.ReadColor();
        ShadowOffsetX = br.ReadInt32();
        ShadowOffsetY = br.ReadInt32();
        CurrentText = br.ReadUInt32();
    }

    public FrontendMultiTextChunk(string name, uint version, int positionX, int positionY, uint dimensionX, uint dimensionY, Justifications justificationX, Justifications justificationY, Color colour, uint translucency, float rotationValue, string textStyleName, byte shadowEnabled, Color shadowColour, int shadowOffsetX, int shadowOffsetY, uint currentText) : base(ChunkID)
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
        TextStyleName = textStyleName;
        ShadowEnabled = shadowEnabled;
        ShadowColour = shadowColour;
        ShadowOffsetX = shadowOffsetX;
        ShadowOffsetY = shadowOffsetY;
        CurrentText = currentText;
    }

    public override void Validate()
    {
        if (TextStyleName == null)
            throw new InvalidDataException($"{nameof(TextStyleName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(TextStyleName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(TextStyleName)} is 255 bytes.");

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
        bw.WriteP3DString(TextStyleName);
        bw.Write(ShadowEnabled);
        bw.Write(ShadowColour);
        bw.Write(ShadowOffsetX);
        bw.Write(ShadowOffsetY);
        bw.Write(CurrentText);
    }
}