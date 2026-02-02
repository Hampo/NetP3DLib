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
        Colours = new(num);
        for (int i = 0; i < num; i++)
            Colours.Add(br.ReadColor());
    }

    public ColourListChunk(IList<Color> colours) : base(ChunkID)
    {
        Colours.AddRange(colours);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumColours)
            yield return new InvalidP3DException(this, $"Num Colours value {NumColours} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumColours);
        foreach (var colour in Colours)
            bw.Write(colour);
    }

    protected override Chunk CloneSelf() => new ColourListChunk(Colours);
}