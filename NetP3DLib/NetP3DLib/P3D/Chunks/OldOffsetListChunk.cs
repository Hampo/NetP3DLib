using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldOffsetListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Offset_List;
    
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
    public uint KeyIndex { get; set; }
    public List<OffsetEntry> Offsets { get; } = [];
    public bool HasPrimGroupIndex { get; set; }
    public uint PrimGroupIndex { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumOffsets));
            data.AddRange(BitConverter.GetBytes(KeyIndex));
            foreach (var offset in Offsets)
                data.AddRange(offset.DataBytes);
            data.AddRange(BitConverter.GetBytes(PrimGroupIndex));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + (uint)Offsets.Sum(x => x.DataBytes.Length) + (HasPrimGroupIndex ? sizeof(uint) : 0u);

    public OldOffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        var numOffsets = br.ReadInt32();
        KeyIndex = br.ReadUInt32();
        Offsets.Capacity = numOffsets;
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(new(br));
        if (br.BaseStream.Position == br.BaseStream.Length)
        {
            HasPrimGroupIndex = false;
            return;
        }
        HasPrimGroupIndex = true;
        PrimGroupIndex = br.ReadUInt32();
    }

    public OldOffsetListChunk(uint keyIndex, IList<OffsetEntry> offsets, uint primGroupIndex) : base(ChunkID)
    {
        KeyIndex = keyIndex;
        Offsets.AddRange(offsets);
        HasPrimGroupIndex = true;
        PrimGroupIndex = primGroupIndex;
    }

    public OldOffsetListChunk(uint keyIndex, IList<OffsetEntry> offsets) : base(ChunkID)
    {
        KeyIndex = keyIndex;
        Offsets.AddRange(offsets);
        HasPrimGroupIndex = false;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumOffsets);
        bw.Write(KeyIndex);
        foreach (var offset in Offsets)
            offset.Write(bw);
        if (HasPrimGroupIndex)
            bw.Write(PrimGroupIndex);
    }

    public class OffsetEntry
    {
        public uint Index { get; set; }
        public Vector3 Offset { get; set; }

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

        public OffsetEntry(BinaryReader br)
        {
            Index = br.ReadUInt32();
            Offset = br.ReadVector3();
        }

        public OffsetEntry(uint index, Vector3 offset)
        {
            Index = index;
            Offset = offset;
        }

        public OffsetEntry()
        {
            Index = 0;
            Offset = Vector3.Zero;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Index);
            bw.Write(Offset);
        }

        public override string ToString()
        {
            return $"{Index} | {Offset}";
        }
    }
}