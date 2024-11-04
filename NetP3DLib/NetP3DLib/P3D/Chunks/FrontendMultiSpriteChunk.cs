using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendMultiSpriteChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Multi_Sprite;
    
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
    public uint NumImageNames
    {
        get => (uint)ImageNames.Count;
        set
        {
            if (value == NumImageNames)
                return;

            if (value < NumImageNames)
            {
                while (NumImageNames > value)
                    ImageNames.RemoveAt(ImageNames.Count - 1);
            }
            else
            {
                while (NumImageNames < value)
                    ImageNames.Add(string.Empty);
            }
        }
    }
    public List<string> ImageNames { get; } = [];

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
            data.AddRange(BitConverter.GetBytes(NumImageNames));
            foreach (var imageName in ImageNames)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(imageName));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(uint) + (uint)ImageNames.Sum(x => BinaryExtensions.GetP3DStringBytes(x).Length);

    public FrontendMultiSpriteChunk(BinaryReader br) : base(ChunkID)
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
        var numImageNames = br.ReadInt32();
        ImageNames.Capacity = numImageNames;
        for (int i = 0; i < numImageNames; i++)
            ImageNames.Add(br.ReadP3DString());
    }

    public FrontendMultiSpriteChunk(string name, uint version, int positionX, int positionY, uint dimensionX, uint dimensionY, Justifications justificationX, Justifications justificationY, Color colour, uint translucency, float rotationValue, IList<string> imageNames) : base(ChunkID)
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
        ImageNames.AddRange(imageNames);
    }

    public override void Validate()
    {
        if (ImageNames.Any(x => x == null || x.Length > 255))
            throw new InvalidDataException($"All {nameof(ImageNames)} must have a value, with a max length of 255 bytes.");

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
        bw.Write(NumImageNames);
        foreach (var imageName in ImageNames)
            bw.WriteP3DString(imageName);
    }
}