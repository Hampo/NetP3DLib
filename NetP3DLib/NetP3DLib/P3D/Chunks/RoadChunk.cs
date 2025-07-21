using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RoadChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Road;
    
    public uint Type { get; set; }
    public string StartIntersection { get; set; }
    public string EndIntersection { get; set; }
    public uint MaximumCars { get; set; }
    public byte Speed { get; set; }
    public byte Intelligence { get; set; }
    public byte Shortcut { get; set; }
    private byte Padding { get; set; }

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
            data.Add(Speed);
            data.Add(Intelligence);
            data.Add(Shortcut);
            data.Add(Padding);

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
        Speed = br.ReadByte();
        Intelligence = br.ReadByte();
        Shortcut = br.ReadByte();
        Padding = br.ReadByte();
    }

    public RoadChunk(string name, uint type, string startIntersection, string endIntersection, uint maximumCars, byte speed, byte intelligence, byte shortcut) : base(ChunkID)
    {
        Name = name;
        Type = type;
        StartIntersection = startIntersection;
        EndIntersection = endIntersection;
        MaximumCars = maximumCars;
        Speed = speed;
        Intelligence = intelligence;
        Shortcut = shortcut;
        Padding = 0;
    }

    public override void Validate()
    {
        if (!StartIntersection.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(StartIntersection), StartIntersection);

        if (!EndIntersection.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(EndIntersection), EndIntersection);

        if (Children.Count == 0)
            throw new InvalidDataException($"There must be at least one Road Segment child chunk.");
        if (Children.Any(x => x.ID != (uint)ChunkIdentifier.Road_Segment))
            throw new InvalidDataException($"Child chunks must be an instance of Road Segment.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.WriteP3DString(StartIntersection);
        bw.WriteP3DString(EndIntersection);
        bw.Write(MaximumCars);
        bw.Write(Speed);
        bw.Write(Intelligence);
        bw.Write(Shortcut);
        bw.Write(Padding);
    }
}