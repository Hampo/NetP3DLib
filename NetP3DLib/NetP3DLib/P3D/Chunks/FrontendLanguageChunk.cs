using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendLanguageChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_Language;
    
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
    public List<Entry> Entries { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.Add((byte)Language);
            data.AddRange(BitConverter.GetBytes(NumEntries));
            data.AddRange(BitConverter.GetBytes(Modulo));

            (List<uint> Hashes, List<uint> Offsets, string Buffer) = BuildData();
            byte[] bufferBytes = Encoding.Unicode.GetBytes(Buffer);

            data.AddRange(BitConverter.GetBytes(bufferBytes.Length));
            foreach (var hash in Hashes)
                data.AddRange(BitConverter.GetBytes(hash));
            foreach (var offset in Offsets)
                data.AddRange(BitConverter.GetBytes(offset));
            data.AddRange(bufferBytes);

            return [.. data];
        }
    }
    public override uint DataLength => (uint)DataBytes.Length;

    public FrontendLanguageChunk(BinaryReader br) : base(ChunkID)
    {
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
        Entries.Capacity = numEntries;
        for (int i = 0; i < numEntries; i++)
        {
            uint hash = hashes[i];
            int offset = (int)offsets[i] / 2;
            int length = buffer.IndexOf('\0', offset) - offset;
            Entries.Add(new(hash, buffer.Substring(offset, length)));
        }
    }

    public FrontendLanguageChunk(string name, char language, uint modulo, IList<Entry> entries) : base(ChunkID)
    {
        Name = name;
        Language = language;
        Modulo = modulo;
        Entries.AddRange(entries);
    }

    public FrontendLanguageChunk(string name, char language, uint modulo, Dictionary<string, string> entries) : base(ChunkID)
    {
        Name = name;
        Language = language;
        Modulo = modulo;
        Entries.Capacity = entries.Count;
        foreach (var entry in entries)
            Entries.Add(new(GetNameHash(entry.Key), entry.Value));
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Language);
        bw.Write(NumEntries);
        bw.Write(Modulo);

        (List<uint> Hashes, List<uint> Offsets, string Buffer) = BuildData();
        byte[] bufferBytes = Encoding.Unicode.GetBytes(Buffer);

        bw.Write(bufferBytes.Length);
        foreach (var hash in Hashes)
            bw.Write(hash);
        foreach (var offset in Offsets)
            bw.Write(offset);
        bw.Write(bufferBytes);
    }

    private (List<uint> Hashes, List<uint> Offsets, string Buffer) BuildData()
    {
        List<uint> hashes = new(Entries.Count);
        List<uint> offsets = new(Entries.Count);
        StringBuilder buffer = new();

        foreach (var entry in Entries)
        {
            hashes.Add(entry.Hash);
            offsets.Add((uint)buffer.Length * 2);
            buffer.Append(entry.Value);
            buffer.Append('\0');
        }

        return (hashes, offsets, buffer.ToString());
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

    public string GetValue(uint hash) => Entries.FirstOrDefault(x => x.Hash == hash)?.Value;

    public string GetValue(string name) => GetValue(GetNameHash(name));

    public void AddValue(uint hash, string value) => Entries.Add(new(hash, value));

    public void AddValue(string name, string value) => AddValue(GetNameHash(name), value);

    public void SetValue(uint hash, string value)
    {
        var index = Entries.FindIndex(x => x.Hash == hash);
        if (index == -1)
            AddValue(hash, value);
        else
            Entries.ElementAt(index).Value = value;
    }

    public void SetValue(string name, string value) => SetValue(GetNameHash(name), value);

    public class Entry
    {
        public uint Hash { get; set; }
        public string Value { get; set; }

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

        public override string ToString()
        {
            return $"0x{Hash:X} | {Value}";
        }
    }
}