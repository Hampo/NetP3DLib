using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ATCChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.ATC;
    
    public uint NumEntries
    {
        get => (uint)Entries.Count;
        set
        {
            if (value == NumEntries)
                return;

            if (value < NumEntries)
            {
                while (NumEntries > value)
                    Entries.RemoveAt(Entries.Count - 1);
            }
            else
            {
                while (NumEntries < value)
                    Entries.Add(new());
            }
        }
    }
    public List<Entry> Entries { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumEntries));
            foreach (var entry in Entries)
                data.AddRange(entry.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)Entries.Sum(x => x.DataBytes.Length);

    public ATCChunk(BinaryReader br) : base(ChunkID)
    {
        var numEntries = br.ReadInt32();
        Entries.Capacity = numEntries;
        for (int i = 0; i < numEntries; i++)
            Entries.Add(new(br));
    }

    public ATCChunk(IList<Entry> entries) : base(ChunkID)
    {
        Entries.AddRange(entries);
    }

    public override void Validate()
    {
        foreach (var entry in Entries)
            entry.Validate();

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumEntries);
        foreach (var entry in Entries)
            entry.Write(bw);
    }

    public class Entry
    {
        public string SoundResourceDataName { get; set; }
        public string Particle { get; set; }
        public string BreakableObject { get; set; }
        public float Friction { get; set; }
        public float Mass { get; set; }
        public float Elasticity { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BinaryExtensions.GetP3DStringBytes(SoundResourceDataName));
                data.AddRange(BinaryExtensions.GetP3DStringBytes(Particle));
                data.AddRange(BinaryExtensions.GetP3DStringBytes(BreakableObject));
                data.AddRange(BitConverter.GetBytes(Friction));
                data.AddRange(BitConverter.GetBytes(Mass));
                data.AddRange(BitConverter.GetBytes(Elasticity));

                return [.. data];
            }
        }

        public Entry(BinaryReader br)
        {
            SoundResourceDataName = br.ReadP3DString();
            Particle = br.ReadP3DString();
            BreakableObject = br.ReadP3DString();
            Friction = br.ReadSingle();
            Mass = br.ReadSingle();
            Elasticity = br.ReadSingle();
        }

        public Entry(string soundResourceDataName, string particle, string breakableObject, float friction, float mass, float elasticity)
        {
            SoundResourceDataName = soundResourceDataName;
            Particle = particle;
            BreakableObject = breakableObject;
            Friction = friction;
            Mass = mass;
            Elasticity = elasticity;
        }

        public Entry()
        {
            SoundResourceDataName = string.Empty;
            Particle = string.Empty;
            BreakableObject = string.Empty;
            Friction = 0;
            Mass = 0;
            Elasticity = 0;
        }

        public void Validate()
        {
            if (SoundResourceDataName == null)
                throw new InvalidDataException($"{nameof(SoundResourceDataName)} cannot be null.");
            if (Encoding.UTF8.GetBytes(SoundResourceDataName).Length > 255)
                throw new InvalidDataException($"The max length of {nameof(SoundResourceDataName)} is 255 bytes.");

            if (Particle == null)
                throw new InvalidDataException($"{nameof(Particle)} cannot be null.");
            if (Encoding.UTF8.GetBytes(Particle).Length > 255)
                throw new InvalidDataException($"The max length of {nameof(Particle)} is 255 bytes.");

            if (BreakableObject == null)
                throw new InvalidDataException($"{nameof(BreakableObject)} cannot be null.");
            if (Encoding.UTF8.GetBytes(BreakableObject).Length > 255)
                throw new InvalidDataException($"The max length of {nameof(BreakableObject)} is 255 bytes.");
        }

        internal void Write(BinaryWriter bw)
        {
            bw.WriteP3DString(SoundResourceDataName);
            bw.WriteP3DString(Particle);
            bw.WriteP3DString(BreakableObject);
            bw.Write(Friction);
            bw.Write(Mass);
            bw.Write(Elasticity);
        }

        public override string ToString()
        {
            return $"{SoundResourceDataName} | {Particle} | {BreakableObject} | {Friction} | {Mass} | {Elasticity}";
        }
    }
}