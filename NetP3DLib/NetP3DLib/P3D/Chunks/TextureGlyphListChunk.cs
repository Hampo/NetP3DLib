using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TextureGlyphListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Texture_Glyph_List;
    
    public uint NumGlyphs
    {
        get => (uint)Glyphs.Count;
        set
        {
            if (value == NumGlyphs)
                return;

            if (value < NumGlyphs)
            {
                while (NumGlyphs > value)
                    Glyphs.RemoveAt(Glyphs.Count - 1);
            }
            else
            {
                while (NumGlyphs < value)
                    Glyphs.Add(new());
            }
        }
    }
    public List<Glyph> Glyphs { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumGlyphs));
            foreach (var glyph in Glyphs)
                data.AddRange(glyph.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)Glyphs.Sum(x => x.DataBytes.Length);

    public TextureGlyphListChunk(BinaryReader br) : base(ChunkID)
    {
        var numGlyphs = br.ReadInt32();
        Glyphs.Capacity = numGlyphs;
        for (int i = 0; i < numGlyphs; i++)
            Glyphs.Add(new(br));
    }

    public TextureGlyphListChunk(IList<Glyph> glyphs) : base(ChunkID)
    {
        Glyphs.AddRange(glyphs);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumGlyphs);
        foreach (var glyph in Glyphs)
            glyph.Write(bw);
    }

    public class Glyph
    {
        public uint TextureNum { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 TopRight { get; set; }
        public float LeftBearing { get; set; }
        public float RightBearing { get; set; }
        public float Width { get; set; }
        public float Advance { get; set; }
        public uint Code { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BitConverter.GetBytes(TextureNum));
                data.AddRange(BinaryExtensions.GetBytes(BottomLeft));
                data.AddRange(BinaryExtensions.GetBytes(TopRight));
                data.AddRange(BitConverter.GetBytes(LeftBearing));
                data.AddRange(BitConverter.GetBytes(RightBearing));
                data.AddRange(BitConverter.GetBytes(Width));
                data.AddRange(BitConverter.GetBytes(Advance));
                data.AddRange(BitConverter.GetBytes(Code));

                return [.. data];
            }
        }

        public Glyph(BinaryReader br)
        {
            TextureNum = br.ReadUInt32();
            BottomLeft = br.ReadVector2();
            TopRight = br.ReadVector2();
            LeftBearing = br.ReadSingle();
            RightBearing = br.ReadSingle();
            Width = br.ReadSingle();
            Advance = br.ReadSingle();
            Code = br.ReadUInt32();
        }

        public Glyph(uint textureNum, Vector2 bottomLeft, Vector2 topRight, float leftBearing, float rightBearing, float width, float advance, uint code)
        {
            TextureNum = textureNum;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            LeftBearing = leftBearing;
            RightBearing = rightBearing;
            Width = width;
            Advance = advance;
            Code = code;
        }

        public Glyph()
        {
            TextureNum = 0;
            BottomLeft = Vector2.Zero;
            TopRight = Vector2.Zero;
            LeftBearing = 0;
            RightBearing = 0;
            Width = 0;
            Advance = 0;
            Code = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(TextureNum);
            bw.Write(BottomLeft);
            bw.Write(TopRight);
            bw.Write(LeftBearing);
            bw.Write(RightBearing);
            bw.Write(Width);
            bw.Write(Advance);
            bw.Write(Code);
        }

        public override string ToString()
        {
            return $"{TextureNum} | {BottomLeft} | {TopRight} | {LeftBearing} | {RightBearing} | {Width} | {Advance} | {Code}";
        }
    }
}