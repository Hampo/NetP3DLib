using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BillboardQuadChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Billboard_Quad;
    
    public uint Version { get; set; }
    public uint CutOffEnabled { get; set; }
    public uint Perspective { get; set; }
    private string _axisMode = string.Empty;
    [MaxLength(4)]
    public string AxisMode
    {
        get => _axisMode;
        set
        {
            if (_axisMode == value)
                return;

            _axisMode = value;
            RecalculateSize();
        }
    }
    public Color Colour { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Distance { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(CutOffEnabled));
            data.AddRange(BitConverter.GetBytes(Perspective));
            data.AddRange(BinaryExtensions.GetFourCCBytes(AxisMode));
            data.AddRange(BinaryExtensions.GetBytes(Colour));
            data.AddRange(BitConverter.GetBytes(Width));
            data.AddRange(BitConverter.GetBytes(Height));
            data.AddRange(BitConverter.GetBytes(Distance));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + 4 + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float);

    public BillboardQuadChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        CutOffEnabled = br.ReadUInt32();
        Perspective = br.ReadUInt32();
        AxisMode = br.ReadFourCC();
        Colour = br.ReadColor();
        Width = br.ReadSingle();
        Height = br.ReadSingle();
        Distance = br.ReadSingle();
    }

    public BillboardQuadChunk(uint version, string name, uint cutOffEnabled, uint perspective, string axisMode, Color colour, float width, float height, float distance) : base(ChunkID)
    {
        Version = version;
        Name = name;
        CutOffEnabled = cutOffEnabled;
        Perspective = perspective;
        AxisMode = axisMode;
        Colour = colour;
        Width = width;
        Height = height;
        Distance = distance;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!AxisMode.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(AxisMode), AxisMode);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(CutOffEnabled);
        bw.Write(Perspective);
        bw.WriteFourCC(AxisMode);
        bw.Write(Colour);
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(Distance);
    }

    protected override Chunk CloneSelf() => new BillboardQuadChunk(Version, Name, CutOffEnabled, Perspective, AxisMode, Colour, Width, Height, Distance);
}