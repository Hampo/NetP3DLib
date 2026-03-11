using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiColourListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Colour_List;

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
        }
    }
    public uint Channel { get; set; }
    public SizeAwareList<Color> Colours { get; }

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

    public MultiColourListChunk(EndianAwareBinaryReader br) : this(ReadChannel(br, out var num), ListHelper.ReadArray(num, br.ReadColor))
    {
    }

    private static uint ReadChannel(EndianAwareBinaryReader br, out int num)
    {
        num = br.ReadInt32();
        return br.ReadUInt32();
    }

    public MultiColourListChunk(uint channel, IList<Color> colours) : base(ChunkID)
    {
        Channel = channel;
        Colours = CreateSizeAwareList(colours);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumColours)
            yield return new InvalidP3DException(this, $"Num Colours value {NumColours} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumColours);
        bw.Write(Channel);
        foreach (var colour in Colours)
            bw.Write(colour);
    }

    protected override Chunk CloneSelf() => new MultiColourListChunk(Channel, Colours);
}