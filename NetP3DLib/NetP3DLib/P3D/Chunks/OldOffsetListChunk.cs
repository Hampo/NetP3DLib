using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldOffsetListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Offset_List;

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
                    Offsets.Add(new());
            }
        }
    }
    public uint KeyIndex { get; set; }
    public SizeAwareList<OffsetEntry> Offsets { get; }
    private bool _hasPrimGroupIndex = false;
    public bool HasPrimGroupIndex
    {
        get => _hasPrimGroupIndex;
        set
        {
            if (_hasPrimGroupIndex == value)
                return;

            var oldSize = HeaderSize;
            _hasPrimGroupIndex = value;
            RecalculateSize(oldSize);
        }
    }
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
            if (HasPrimGroupIndex)
                data.AddRange(BitConverter.GetBytes(PrimGroupIndex));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + OffsetEntry.Size * NumOffsets + (HasPrimGroupIndex ? sizeof(uint) : 0u);

    // TODO: Implement `PrimGroupIndex`
    public OldOffsetListChunk(EndianAwareBinaryReader br) : this(ReadKeyIndex(br, out var numOffsets), ListHelper.ReadArray(numOffsets, () => new OffsetEntry(br)), br.ReadUInt32())
    {
    }

    private static uint ReadKeyIndex(EndianAwareBinaryReader br, out int numOffsets)
    {
        numOffsets = br.ReadInt32();
        return br.ReadUInt32();
    }

    public OldOffsetListChunk(uint keyIndex, IList<OffsetEntry> offsets) : this(keyIndex, offsets, null)
    {
    }

    public OldOffsetListChunk(uint keyIndex, IList<OffsetEntry> offsets, uint? primGroupIndex = null) : base(ChunkID)
    {
        KeyIndex = keyIndex;
        Offsets = CreateSizeAwareList(offsets);
        HasPrimGroupIndex = primGroupIndex.HasValue;
        if (HasPrimGroupIndex)
            PrimGroupIndex = primGroupIndex!.Value;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Children.Count != GetChildCount(ChunkIdentifier.Old_Index_Offset_List))
            yield return new InvalidP3DException(this, $"Invalid children. Must be only {nameof(OldIndexOffsetListChunk)}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumOffsets);
        bw.Write(KeyIndex);
        foreach (var offset in Offsets)
            offset.Write(bw);
        if (HasPrimGroupIndex)
            bw.Write(PrimGroupIndex);
    }

    protected override Chunk CloneSelf()
    {
        var offsets = new OffsetEntry[Offsets.Count];
        for (var i = 0; i < Offsets.Count; i++)
            offsets[i] = Offsets[i].Clone();
        return new OldOffsetListChunk(KeyIndex, offsets);
    }

    public class OffsetEntry
    {
        public const uint Size = sizeof(uint) + sizeof(float) * 3;

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

        internal OffsetEntry Clone() => new(Index, Offset);

        public override string ToString() => $"{Index} | {Offset}";
    }
}