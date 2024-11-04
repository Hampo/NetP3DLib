using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ColourListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Colour_List;
    
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
        }
    }
    public List<Color> Colours { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumColours));
            foreach (var colour in Colours)
                data.AddRange(BinaryExtensions.GetBytes(colour));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) * NumColours;

    public ColourListChunk(BinaryReader br) : base(ChunkID)
    {
        var num = br.ReadInt32();
        Colours.Capacity = num;
        for (int i = 0; i < num; i++)
            Colours.Add(br.ReadColor());
    }

    public ColourListChunk(IList<Color> colours) : base(ChunkID)
    {
        Colours.AddRange(colours);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumColours);
        foreach (var colour in Colours)
            bw.Write(colour);
    }
}