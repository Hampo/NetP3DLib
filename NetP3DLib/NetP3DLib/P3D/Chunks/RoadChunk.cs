using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            data.Add(0);

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(StartIntersection).Length + (uint)BinaryExtensions.GetP3DStringBytes(EndIntersection).Length + sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(byte);

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
        var padding = br.ReadByte();
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
    }

    public override void Validate()
    {
        if (StartIntersection == null)
            throw new InvalidDataException($"{nameof(StartIntersection)} cannot be null.");
        if (Encoding.UTF8.GetBytes(StartIntersection).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(StartIntersection)} is 255 bytes.");

        if (EndIntersection == null)
            throw new InvalidDataException($"{nameof(EndIntersection)} cannot be null.");
        if (Encoding.UTF8.GetBytes(EndIntersection).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(EndIntersection)} is 255 bytes.");

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
        bw.Write((byte)0);
    }
}