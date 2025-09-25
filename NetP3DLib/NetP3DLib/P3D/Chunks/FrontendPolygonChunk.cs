using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPolygonChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Polygon;
    
    public uint Version { get; set; }
    public uint Translucency { get; set; }
    public uint NumPoints
    {
        get => (uint)Points.Count;
        set
        {
            if (value == NumPoints)
                return;

            if (value < NumPoints)
            {
                while (NumPoints > value)
                    Points.RemoveAt(Points.Count - 1);
            }
            else
            {
                while (NumPoints < value)
                    Points.Add(default);
            }
            NumColours = value;
        }
    }
    public List<Vector3> Points { get; } = [];
    public uint NumColours
    {
        get => (uint)Colours.Count;
        set
        {
            if (value == NumColours)
                return;

            if (value < NumColours)
            {
                while (NumColours > value)
                    Colours.RemoveAt(Colours.Count - 1);
            }
            else
            {
                while (NumColours < value)
                    Colours.Add(default);
            }
            NumPoints = value;
        }
    }
    public List<Color> Colours { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Translucency));
            data.AddRange(BitConverter.GetBytes(NumPoints));
            foreach (var point in Points)
                data.AddRange(BinaryExtensions.GetBytes(point));
            foreach (var colour in Colours)
                data.AddRange(BinaryExtensions.GetBytes(colour));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) * 3 * NumPoints + sizeof(uint) * NumColours;

    public FrontendPolygonChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Translucency = br.ReadUInt32();
        var num = br.ReadInt32();
        Points = new(num);
        for (int i = 0; i < num; i++)
            Points.Add(br.ReadVector3());
        Colours = new(num);
        for (int i = 0; i < num; i++)
            Colours.Add(br.ReadColor());
    }

    public FrontendPolygonChunk(string name, uint version, uint translucency, IList<Vector3> points, IList<Color> colours) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Translucency = translucency;
        Points.AddRange(points);
        Colours.AddRange(colours);
    }

    public override void Validate()
    {
        if (Points.Count != Colours.Count)
            throw new InvalidDataException($"{nameof(Points)} and {nameof(Colours)} must have equal counts.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Translucency);
        bw.Write(NumPoints);
        foreach (var point in Points)
            bw.Write(point);
        foreach (var colour in Colours)
            bw.Write(colour);
    }

    internal override Chunk CloneSelf() => new FrontendPolygonChunk(Name, Version, Translucency, Points, Colours);
}