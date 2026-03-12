using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendMultiTextChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Multi_Text;

    public enum Justifications : uint
    {
        Left,
        Right,
        Top,
        Bottom,
        Centre
    }

    [DefaultValue(17)]
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
    private readonly P3DString _textStyleName;
    public string TextStyleName
    {
        get => _textStyleName?.Value ?? string.Empty;
        set => _textStyleName.Value = value;
    }
    private byte _shadowEnabled;
    public bool ShadowEnabled
    {
        get => _shadowEnabled != 0;
        set => _shadowEnabled = (byte)(value ? 1 : 0);
    }
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
            data.Add(_shadowEnabled);
            data.AddRange(BinaryExtensions.GetBytes(ShadowColour));
            data.AddRange(BitConverter.GetBytes(ShadowOffsetX));
            data.AddRange(BitConverter.GetBytes(ShadowOffsetY));
            data.AddRange(BitConverter.GetBytes(CurrentText));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + BinaryExtensions.GetP3DStringLength(TextStyleName) + sizeof(byte) + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint);

    public FrontendMultiTextChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadUInt32(), br.ReadUInt32(), (Justifications)br.ReadUInt32(), (Justifications)br.ReadUInt32(), br.ReadColor(), br.ReadUInt32(), br.ReadSingle(), br.ReadP3DString(), br.ReadByte(), br.ReadColor(), br.ReadInt32(), br.ReadInt32(), br.ReadUInt32())
    {
    }

    public FrontendMultiTextChunk(string name, uint version, int positionX, int positionY, uint dimensionX, uint dimensionY, Justifications justificationX, Justifications justificationY, Color colour, uint translucency, float rotationValue, string textStyleName, bool shadowEnabled, Color shadowColour, int shadowOffsetX, int shadowOffsetY, uint currentText) : this(name, version, positionX, positionY, dimensionX, dimensionY, justificationX, justificationY, colour, translucency, rotationValue, textStyleName, (byte)(shadowEnabled ? 1 : 0), shadowColour, shadowOffsetX, shadowOffsetY, currentText)
    {
    }

    public FrontendMultiTextChunk(string name, uint version, int positionX, int positionY, uint dimensionX, uint dimensionY, Justifications justificationX, Justifications justificationY, Color colour, uint translucency, float rotationValue, string textStyleName, byte shadowEnabled, Color shadowColour, int shadowOffsetX, int shadowOffsetY, uint currentText) : base(ChunkID, name)
    {
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
        _textStyleName = new(this, textStyleName, nameof(TextStyleName));
        _shadowEnabled = shadowEnabled;
        ShadowColour = shadowColour;
        ShadowOffsetX = shadowOffsetX;
        ShadowOffsetY = shadowOffsetY;
        CurrentText = currentText;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!TextStyleName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(TextStyleName), TextStyleName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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
        bw.Write(_shadowEnabled);
        bw.Write(ShadowColour);
        bw.Write(ShadowOffsetX);
        bw.Write(ShadowOffsetY);
        bw.Write(CurrentText);
    }

    protected override Chunk CloneSelf() => new FrontendMultiTextChunk(Name, Version, PositionX, PositionY, DimensionX, DimensionY, JustificationX, JustificationY, Colour, Translucency, RotationValue, TextStyleName, ShadowEnabled, ShadowColour, ShadowOffsetX, ShadowOffsetY, CurrentText);
}
