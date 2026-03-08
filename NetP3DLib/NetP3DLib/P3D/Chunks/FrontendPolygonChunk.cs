using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendPolygonChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Polygon;

    [DefaultValue(1)]
    public uint Version { get; set; }
    public uint Translucency { get; set; }
    public uint NumPoints
    {
        get => (uint)(Points?.Count ?? 0);
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
    public SizeAwareList<Vector3> Points { get; }
    public uint NumColours
    {
        get => (uint)(Colours?.Count ?? 0);
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
    public SizeAwareList<Color> Colours { get; }

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

    public FrontendPolygonChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        Version = br.ReadUInt32();
        Translucency = br.ReadUInt32();
        var num = br.ReadInt32();
        var points = new Vector3[num];
        for (int i = 0; i < num; i++)
            points[i] = br.ReadVector3();
        Points = CreateSizeAwareList(points);
        var colours = new Color[num];
        for (int i = 0; i < num; i++)
            colours[i] = br.ReadColor();
        Colours = CreateSizeAwareList(colours);
    }

    public FrontendPolygonChunk(string name, uint version, uint translucency, IList<Vector3> points, IList<Color> colours) : base(ChunkID)
    {
        _name = new(this, name);
        Version = version;
        Translucency = translucency;
        Points = CreateSizeAwareList(points);
        Colours = CreateSizeAwareList(colours);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Points.Count != Colours.Count)
            yield return new InvalidP3DException(this, $"{nameof(Points)} and {nameof(Colours)} must have equal counts.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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

    protected override Chunk CloneSelf() => new FrontendPolygonChunk(Name, Version, Translucency, Points, Colours);
}
