using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RoadChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Road;
    private const uint SpeedMask = 0x000000FF;
    private const uint IntelligenceMask = 0x0000FF00;
    private const uint ShortcutMask = 0x00010000;

    public uint Type { get; set; }
    private string _startIntersection = string.Empty;
    public string StartIntersection
    {
        get => _startIntersection;
        set
        {
            if (_startIntersection == value)
                return;

            _startIntersection = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private string _endIntersection = string.Empty;
    public string EndIntersection
    {
        get => _endIntersection;
        set
        {
            if (_endIntersection == value)
                return;

            _endIntersection = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public uint MaximumCars { get; set; }
    private uint bitmask;
    public byte Speed
    {
        get => (byte)(bitmask & SpeedMask);
        set
        {
            bitmask &= ~SpeedMask;
            bitmask |= value;
        }
    }
    public byte Intelligence
    {
        get => (byte)((bitmask & IntelligenceMask) >> 8);
        set
        {
            bitmask &= ~IntelligenceMask;
            bitmask |= (uint)(value << 8);
        }
    }
    public bool Shortcut
    {
        get => (bitmask & ShortcutMask) != 0;
        set
        {
            if (value)
                bitmask |= ShortcutMask;
            else
                bitmask &= ~ShortcutMask;
        }
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Type));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(StartIntersection));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(EndIntersection));
            data.AddRange(BitConverter.GetBytes(MaximumCars));
            data.AddRange(BitConverter.GetBytes(bitmask));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(StartIntersection) + BinaryExtensions.GetP3DStringLength(EndIntersection) + sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(byte);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public RoadChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        StartIntersection = br.ReadP3DString();
        EndIntersection = br.ReadP3DString();
        MaximumCars = br.ReadUInt32();
        bitmask = br.ReadUInt32();
    }

    public RoadChunk(string name, uint type, string startIntersection, string endIntersection, uint maximumCars, byte speed, byte intelligence, bool shortcut) : base(ChunkID)
    {
        Name = name;
        Type = type;
        StartIntersection = startIntersection;
        EndIntersection = endIntersection;
        MaximumCars = maximumCars;
        bitmask = 0;
        Speed = speed;
        Intelligence = intelligence;
        Shortcut = shortcut;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (!StartIntersection.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(StartIntersection), StartIntersection);

        if (!EndIntersection.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(EndIntersection), EndIntersection);

        if (Children.Count == 0)
            yield return new InvalidP3DException(this, $"There must be at least one Road Segment child chunk.");
        foreach (var child in Children)
            if (child.ID != (uint)ChunkIdentifier.Road_Segment)
                yield return new InvalidP3DException(this, $"Child chunk {child} is invalid. Child chunks must be an instance of Road Segment.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.WriteP3DString(StartIntersection);
        bw.WriteP3DString(EndIntersection);
        bw.Write(MaximumCars);
        bw.Write(bitmask);
    }

    protected override Chunk CloneSelf() => new RoadChunk(Name, Type, StartIntersection, EndIntersection, MaximumCars, Speed, Intelligence, Shortcut);
}