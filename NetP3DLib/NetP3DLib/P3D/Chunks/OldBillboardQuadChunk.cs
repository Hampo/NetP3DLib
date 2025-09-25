using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardQuadChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Quad;
    
    [DefaultValue(2)]
    public uint Version { get; set; }
    public string BillboardMode { get; set; }
    public Vector3 Translation { get; set; }
    public Color Colour { get; set; }
    public Vector2 UV0 { get; set; }
    public Vector2 UV1 { get; set; }
    public Vector2 UV2 { get; set; }
    public Vector2 UV3 { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Distance { get; set; }
    public Vector2 UVOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(BillboardMode));
            data.AddRange(BinaryExtensions.GetBytes(Translation));
            data.AddRange(BinaryExtensions.GetBytes(Colour));
            data.AddRange(BinaryExtensions.GetBytes(UV0));
            data.AddRange(BinaryExtensions.GetBytes(UV1));
            data.AddRange(BinaryExtensions.GetBytes(UV2));
            data.AddRange(BinaryExtensions.GetBytes(UV3));
            data.AddRange(BitConverter.GetBytes(Width));
            data.AddRange(BitConverter.GetBytes(Height));
            data.AddRange(BitConverter.GetBytes(Distance));
            data.AddRange(BinaryExtensions.GetBytes(UVOffset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4 + sizeof(float) * 3 + sizeof(uint) + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) * 2;

    public OldBillboardQuadChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        BillboardMode = br.ReadFourCC();
        Translation = br.ReadVector3();
        Colour = br.ReadColor();
        UV0 = br.ReadVector2();
        UV1 = br.ReadVector2();
        UV2 = br.ReadVector2();
        UV3 = br.ReadVector2();
        Width = br.ReadSingle();
        Height = br.ReadSingle();
        Distance = br.ReadSingle();
        UVOffset = br.ReadVector2();
    }

    public OldBillboardQuadChunk(uint version, string name, string billboardMode, Vector3 translation, Color colour, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, float width, float height, float distance, Vector2 uvOffset) : base(ChunkID)
    {
        Version = version;
        Name = name;
        BillboardMode = billboardMode;
        Translation = translation;
        Colour = colour;
        UV0 = uv0;
        UV1 = uv1;
        UV2 = uv2;
        UV3 = uv3;
        Width = width;
        Height = height;
        Distance = distance;
        UVOffset = uvOffset;
    }

    public override void Validate()
    {
        if (!BillboardMode.IsValidFourCC())
            throw new InvalidFourCCException(nameof(BillboardMode), BillboardMode);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(BillboardMode);
        bw.Write(Translation);
        bw.Write(Colour);
        bw.Write(UV0);
        bw.Write(UV1);
        bw.Write(UV2);
        bw.Write(UV3);
        bw.Write(Width);
        bw.Write(Height);
        bw.Write(Distance);
        bw.Write(UVOffset);
    }

    internal override Chunk CloneSelf() => new OldBillboardQuadChunk(Version, Name, BillboardMode, Translation, Colour, UV0, UV1, UV2, UV3, Width, Height, Distance, UVOffset);
}
