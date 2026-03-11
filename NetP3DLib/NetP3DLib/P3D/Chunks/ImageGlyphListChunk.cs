using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageGlyphListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Image_Glyph_List;

    public uint NumGlyphs
    {
        get => (uint)(Glyphs?.Count ?? 0);
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
    public SizeAwareList<Glyph> Glyphs { get; }

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
    public override uint DataLength => sizeof(uint) + Glyph.Size * NumGlyphs;

    public ImageGlyphListChunk(EndianAwareBinaryReader br) : this(ListHelper.ReadArray(br.ReadInt32(), () => new Glyph(br)))
    {
    }

    public ImageGlyphListChunk(IList<Glyph> glyphs) : base(ChunkID)
    {
        Glyphs = CreateSizeAwareList(glyphs);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumGlyphs);
        foreach (var glyph in Glyphs)
            glyph.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var glyphs = new Glyph[Glyphs.Count];
        for (var i = 0; i < Glyphs.Count; i++)
            glyphs[i] = Glyphs[i].Clone();
        return new ImageGlyphListChunk(glyphs);
    }

    public class Glyph
    {
        public const uint Size = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint);

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

        internal Glyph Clone() => new(XOrigin, LeftBearing, RightBearing, Width, Advance, Code);

        public override string ToString() => $"{XOrigin} | {LeftBearing} | {RightBearing} | {Width} | {Advance} | {Code}";
    }
}