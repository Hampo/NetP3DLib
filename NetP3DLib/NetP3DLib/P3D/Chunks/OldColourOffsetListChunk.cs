using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldColourOffsetListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Colour_Offset_List;
    
    public uint Version { get; set; }
    public uint NumOffsets
    {
        get => (uint)(Offsets?.Count ?? 0);
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
    public SizeAwareList<Color> Offsets { get; }

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

    public OldColourOffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numOffsets = br.ReadInt32();
        var offsets = new Color[numOffsets];
        for (int i = 0; i < numOffsets; i++)
            offsets[i] = br.ReadColor();
        Offsets = CreateSizeAwareList(offsets);
    }

    public OldColourOffsetListChunk(uint version, IList<Color> offsets) : base(ChunkID)
    {
        Version = version;
        Offsets = CreateSizeAwareList(offsets);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumOffsets);
        foreach (var offset in Offsets)
            bw.Write(offset);
    }

    protected override Chunk CloneSelf() => new OldColourOffsetListChunk(Version, Offsets);
}