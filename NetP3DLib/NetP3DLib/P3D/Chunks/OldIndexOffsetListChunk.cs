using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldIndexOffsetListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Index_Offset_List;
    
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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<uint> Offsets { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumOffsets));
            foreach (var offset in Offsets)
                data.AddRange(BitConverter.GetBytes(offset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) * NumOffsets;

    public OldIndexOffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numOffsets = br.ReadInt32();
        Offsets = new(numOffsets);
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(br.ReadUInt32());
    }

    public OldIndexOffsetListChunk(uint version, IList<uint> offsets) : base(ChunkID)
    {
        Version = version;
        Offsets.AddRange(offsets);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumOffsets);
        foreach (var offset in Offsets)
            bw.Write(offset);
    }

    protected override Chunk CloneSelf() => new OldIndexOffsetListChunk(Version, Offsets);
}