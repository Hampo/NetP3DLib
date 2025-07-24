using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiColourListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Colour_List;
    
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
    public uint Channel { get; set; }
    public List<Color> Colours { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumColours));
            data.AddRange(BitConverter.GetBytes(Channel));
            foreach (var colour in Colours)
                data.AddRange(BinaryExtensions.GetBytes(colour));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) * NumColours;

    public MultiColourListChunk(BinaryReader br) : base(ChunkID)
    {
        var num = br.ReadInt32();
        Channel = br.ReadUInt32();
        Colours = new(num);
        for (int i = 0; i < num; i++)
            Colours.Add(br.ReadColor());
    }

    public MultiColourListChunk(uint channel, IList<Color> colours) : base(ChunkID)
    {
        Channel = channel;
        Colours.AddRange(colours);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumColours);
        bw.Write(Channel);
        foreach (var colour in Colours)
            bw.Write(colour);
    }

    internal override Chunk CloneSelf() => new MultiColourListChunk(Channel, Colours);
}