using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendLanguageChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Language;
    
    public char Language { get; set; }
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
    public uint Modulo { get; set; }
    public SizeAwareList<Entry> Entries { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.Add((byte)Language);
            data.AddRange(BitConverter.GetBytes(NumEntries));
            data.AddRange(BitConverter.GetBytes(Modulo));

            (List<uint> Hashes, List<uint> Offsets, List<ushort> Buffer) = BuildData();

            data.AddRange(BitConverter.GetBytes(Buffer.Count * 2));
            foreach (var hash in Hashes)
                data.AddRange(BitConverter.GetBytes(hash));
            foreach (var offset in Offsets)
                data.AddRange(BitConverter.GetBytes(offset));
            foreach (var code in Buffer)
                data.AddRange(BitConverter.GetBytes(code));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + 1 + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) * (uint)Entries.Count + sizeof(uint) * (uint)Entries.Count + (uint)(BuildData().Buffer.Count * 2);//(uint)DataBytes.Length;

    public FrontendLanguageChunk(BinaryReader br) : base(ChunkID)
    {
        Entries = CreateSizeAwareList<Entry>();
        Name = br.ReadP3DString();
        Language = br.ReadChar();
        var numEntries = br.ReadInt32();
        Modulo = br.ReadUInt32();
        var bufferSize = br.ReadInt32();
        List<uint> hashes = new(numEntries);
        for (int i = 0; i < numEntries; i++)
            hashes.Add(br.ReadUInt32());
        List<uint> offsets = new(numEntries);
        for (int i = 0; i < numEntries; i++)
            offsets.Add(br.ReadUInt32());
        byte[] bufferBytes = br.ReadBytes(bufferSize);
        string buffer = Encoding.Unicode.GetString(bufferBytes);
        Entries = CreateSizeAwareList<Entry>(numEntries);
        Entries.CollectionChanged += Entries_CollectionChanged;
        Entries.SuspendNotifications();
        for (int i = 0; i < numEntries; i++)
        {
            uint hash = hashes[i];
            int offset = (int)offsets[i] / 2;
            int length = buffer.IndexOf('\0', offset) - offset;
            Entries.Add(new(hash, buffer.Substring(offset, length)));
        }
        Entries.ResumeNotifications();
    }

    public FrontendLanguageChunk(string name, char language, uint modulo, IList<Entry> entries) : base(ChunkID)
    {
        Entries = CreateSizeAwareList<Entry>(entries.Count);
        Name = name;
        Language = language;
        Modulo = modulo;
        Entries.CollectionChanged += Entries_CollectionChanged;

        Entries.SuspendNotifications();
        Entries.AddRange(entries);
        Entries.ResumeNotifications();
    }

    public FrontendLanguageChunk(string name, char language, uint modulo, Dictionary<string, string> entries) : base(ChunkID)
    {
        Entries = CreateSizeAwareList<Entry>(entries.Count);
        Name = name;
        Language = language;
        Modulo = modulo;
        Entries.CollectionChanged += Entries_CollectionChanged;
        Entries.SuspendNotifications();
        foreach (var entry in entries)
            Entries.Add(new(GetNameHash(entry.Key), entry.Value));
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

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Language);
        bw.Write(NumEntries);
        bw.Write(Modulo);

        (List<uint> Hashes, List<uint> Offsets, List<ushort> Buffer) = BuildData();

        bw.Write(Buffer.Count * 2);
        foreach (var hash in Hashes)
            bw.Write(hash);
        foreach (var offset in Offsets)
            bw.Write(offset);
        foreach (var code in Buffer)
            bw.Write(code);
    }

    protected override Chunk CloneSelf()
    {
        var entries = new List<Entry>(Entries.Count);
        foreach (var entry in Entries)
            entries.Add(entry.Clone());
        return new FrontendLanguageChunk(Name, Language, Modulo, entries);
    }

    private (List<uint> Hashes, List<uint> Offsets, List<ushort> Buffer) BuildData()
    {
        List<uint> hashes = new(Entries.Count);
        List<uint> offsets = new(Entries.Count);
        StringBuilder sb = new();

        foreach (var entry in Entries)
        {
            hashes.Add(entry.Hash);
            offsets.Add((uint)sb.Length * 2);
            sb.Append(entry.Value);
            sb.Append('\0');
        }

        var bufferBytes = Encoding.Unicode.GetBytes(sb.ToString());
        List<ushort> buffer = new(bufferBytes.Length / 2);
        for (int i = 0; i < bufferBytes.Length; i += 2)
            buffer.Add(BitConverter.ToUInt16(bufferBytes, i));

        return (hashes, offsets, buffer);
    }

    public uint GetNameHash(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), $"{nameof(name)} cannot be null or empty.");

        uint Hash = 0;
        foreach (var c in name)
        {
            int i = Convert.ToInt32(c);
            Hash = (uint)((i + (Hash << 6)) % Modulo);
        }
        return Hash;
    }

    public string? GetValue(uint hash)
    {
        foreach (var entry in Entries)
            if (entry.Hash == hash)
                return entry.Value;
        return null;
    }

    public string? GetValue(string name) => GetValue(GetNameHash(name));

    public void AddValue(uint hash, string value) => Entries.Add(new(hash, value));

    public void AddValue(string name, string value) => AddValue(GetNameHash(name), value);

    public void SetValue(uint hash, string value)
    {
        var index = Entries.FindIndex(x => x.Hash == hash);
        if (index == -1)
            AddValue(hash, value);
        else
            Entries[index].Value = value;
    }

    public void SetValue(string name, string value) => SetValue(GetNameHash(name), value);

    public class Entry
    {
        public event Action? SizeChanged;

        public uint Hash { get; set; }
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                SizeChanged?.Invoke();
            }
        }

        public Entry(uint hash, string value)
        {
            Hash = hash;
            Value = value;
        }

        public Entry(uint hash)
        {
            Hash = hash;
            Value = string.Empty;
        }

        public Entry()
        {
            Hash = 0;
            Value = string.Empty;
        }

        public Entry Clone() => new(Hash, Value);

        public override string ToString() => $"0x{Hash:X} | {Value}";
    }
}