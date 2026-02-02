using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class Vector2OffsetListChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vector2_Offset_List;
    
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
                    Offsets.Add(new());
            }
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<Vector2Offset> Offsets { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            data.AddRange(BitConverter.GetBytes(NumOffsets));
            foreach (var offset in Offsets)
                data.AddRange(offset.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + 4 + sizeof(uint) + Vector2Offset.Size * NumOffsets;

    public Vector2OffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Param = br.ReadFourCC();
        var numOffsets = br.ReadInt32();
        Offsets = new(numOffsets);
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(new(br));
    }

    public Vector2OffsetListChunk(uint version, string param, IList<Vector2Offset> offsets) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Offsets.AddRange(offsets);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteFourCC(Param);
        bw.Write(NumOffsets);
        foreach (var offset in Offsets)
            offset.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var offsets = new List<Vector2Offset>(Offsets.Count);
        foreach (var offset in Offsets)
            offsets.Add(offset.Clone());
        return new Vector2OffsetListChunk(Version, Param, offsets);
    }

    public class Vector2Offset
    {
        public const uint Size = sizeof(uint) + sizeof(float) * 2;

        public uint Index;
        public Vector2 Offset;

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BitConverter.GetBytes(Index));
                data.AddRange(BinaryExtensions.GetBytes(Offset));

                return [.. data];
            }
        }

        public Vector2Offset(BinaryReader br)
        {
            Index = br.ReadUInt32();
            Offset = br.ReadVector2();
        }

        public Vector2Offset(uint index, Vector2 offset)
        {
            Index = index;
            Offset = offset;
        }

        public Vector2Offset()
        {
            Index = 0;
            Offset = new();
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Index);
            bw.Write(Offset);
        }

        internal Vector2Offset Clone() => new(Index, Offset);

        public override string ToString() => $"{Index} | {Offset}";
    }
}