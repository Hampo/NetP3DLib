using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ATCChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.ATC;
    
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
    public SizeAwareList<Entry> Entries { get; }

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
    public override uint DataLength
    {
        get
        {
            uint size = sizeof(uint);
            foreach (var entry in Entries)
                size += (uint)entry.DataBytes.Length;
            return size;
        }
    }

    public ATCChunk(BinaryReader br) : base(ChunkID)
    {
        var numEntries = br.ReadInt32();
        Entries = CreateSizeAwareList<Entry>(numEntries);
        Entries.SuspendNotifications();
        Entries.CollectionChanged += Entries_CollectionChanged;

        for (int i = 0; i < numEntries; i++)
            Entries.Add(new(br));
        Entries.ResumeNotifications();
    }

    public ATCChunk(IList<Entry> entries) : base(ChunkID)
    {
        Entries = CreateSizeAwareList<Entry>(entries.Count);
        Entries.CollectionChanged += Entries_CollectionChanged;

        Entries.SuspendNotifications();
        Entries.AddRange(entries);
        Entries.ResumeNotifications();
    }

    private void Entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (Entry oldEntry in e.OldItems)
                oldEntry.SizeChanged -= Entry_SizeChanged;

        if (e.NewItems != null)
            foreach (Entry newEntry in e.NewItems)
                newEntry.SizeChanged += Entry_SizeChanged;

        int delta = checked((int)(Size - _cachedSize));
        _cachedSize = Size;
        OnSizeChanged(delta);
    }

    private void Entry_SizeChanged()
    {
        int delta = checked((int)(Size - _cachedSize));
        _cachedSize = Size;
        OnSizeChanged(delta);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        foreach (var entry in Entries)
            foreach (var error in entry.Validate(this))
                yield return error;

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumEntries);
        foreach (var entry in Entries)
            entry.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var entries = new List<Entry>(Entries.Count);
        foreach (var entry in Entries)
            entries.Add(entry.Clone());
        return new ATCChunk(entries);
    }

    public class Entry
    {
        public event Action? SizeChanged;

        private string _soundResourceDataName = string.Empty;
        public string SoundResourceDataName
        {
            get => _soundResourceDataName;
            set
            {
                if (_soundResourceDataName == value) return;
                _soundResourceDataName = value;
                SizeChanged?.Invoke();
            }
        }

        private string _particle = string.Empty;
        public string Particle
        {
            get => _particle;
            set
            {
                if (_particle == value) return;
                _particle = value;
                SizeChanged?.Invoke();
            }
        }

        private string _breakableObject = string.Empty;
        public string BreakableObject
        {
            get => _breakableObject;
            set
            {
                if (_breakableObject == value) return;
                _breakableObject = value;
                SizeChanged?.Invoke();
            }
        }
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

        public IEnumerable<InvalidP3DException> Validate(ATCChunk chunk)
        {
            if (!SoundResourceDataName.IsValidP3DString())
                yield return new InvalidP3DStringException(chunk, nameof(SoundResourceDataName), SoundResourceDataName);

            if (!Particle.IsValidP3DString())
                yield return new InvalidP3DStringException(chunk, nameof(Particle), Particle);

            if (!BreakableObject.IsValidP3DString())
                yield return new InvalidP3DStringException(chunk, nameof(BreakableObject), BreakableObject);
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

        internal Entry Clone() => new(SoundResourceDataName, Particle, BreakableObject, Friction, Mass, Elasticity);

        public override string ToString() => $"{SoundResourceDataName} | {Particle} | {BreakableObject} | {Friction} | {Mass} | {Elasticity}";
    }
}