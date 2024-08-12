using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Colour_Offset_List)]
public class OldColourOffsetListChunk : Chunk
{
    public uint Version { get; set; }
    public uint NumOffsets
    {
        get => (uint)Offsets.Count;
        set
        {
            if (value == NumOffsets)
                return;

            if (value < NumOffsets)
            {
                while (NumOffsets > value)
                    Offsets.RemoveAt(Offsets.Count - 1);
            }
            else
            {
                while (NumOffsets < value)
                    Offsets.Add(default);
            }
        }
    }
    public List<Color> Offsets { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumOffsets));
            foreach (var offset in Offsets)
                data.AddRange(BinaryExtensions.GetBytes(offset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) * NumOffsets;

    public OldColourOffsetListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Colour_Offset_List)
    {
        Version = br.ReadUInt32();
        var numOffsets = br.ReadInt32();
        Offsets.Capacity = numOffsets;
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(br.ReadColor());
    }

    public OldColourOffsetListChunk(uint version, IList<Color> offsets) : base((uint)ChunkIdentifier.Old_Colour_Offset_List)
    {
        Version = version;
        Offsets.AddRange(offsets);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumOffsets);
        foreach (var offset in Offsets)
            bw.Write(offset);
    }
}