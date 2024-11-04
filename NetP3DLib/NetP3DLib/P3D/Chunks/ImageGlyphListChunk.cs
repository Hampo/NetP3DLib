using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageGlyphListChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Image_Glyph_List;
    
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

    public ImageGlyphListChunk(BinaryReader br) : base(ChunkID)
    {
        var numGlyphs = br.ReadInt32();
        Glyphs.Capacity = numGlyphs;
        for (int i = 0; i < numGlyphs; i++)
            Glyphs.Add(new(br));
    }

    public ImageGlyphListChunk(IList<Glyph> glyphs) : base(ChunkID)
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
        public ushort XOrigin { get; set; }
        public ushort LeftBearing { get; set; }
        public ushort RightBearing { get; set; }
        public ushort Width { get; set; }
        public ushort Advance { get; set; }
        public uint Code { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BitConverter.GetBytes(XOrigin));
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
            XOrigin = br.ReadUInt16();
            LeftBearing = br.ReadUInt16();
            RightBearing = br.ReadUInt16();
            Width = br.ReadUInt16();
            Advance = br.ReadUInt16();
            Code = br.ReadUInt32();
        }

        public Glyph(ushort xOrigin, ushort leftBearing, ushort rightBearing, ushort width, ushort advance, uint code)
        {
            XOrigin = xOrigin;
            LeftBearing = leftBearing;
            RightBearing = rightBearing;
            Width = width;
            Advance = advance;
            Code = code;
        }

        public Glyph()
        {
            XOrigin = 0;
            LeftBearing = 0;
            RightBearing = 0;
            Width = 0;
            Advance = 0;
            Code = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(XOrigin);
            bw.Write(LeftBearing);
            bw.Write(RightBearing);
            bw.Write(Width);
            bw.Write(Advance);
            bw.Write(Code);
        }

        public override string ToString()
        {
            return $"{XOrigin} | {LeftBearing} | {RightBearing} | {Width} | {Advance} | {Code}";
        }
    }
}