using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Expression;

    private uint _version;
    [DefaultValue(0)]
    public uint Version
    {
        get => _version;
        set
        {
            if (_version == value)
                return;
    
            _version = value;
            OnPropertyChanged(nameof(Version));
        }
    }
    
    public uint NumKeys
    {
        get => (uint)(Keys?.Count ?? 0);
        set
        {
            if (value == NumKeys)
                return;

            if (value < NumKeys)
            {
                while (NumKeys > value)
                    Keys.RemoveAt(Keys.Count - 1);
            }
            else
            {
                while (NumKeys < value)
                    Keys.Add(default);
            }
            NumIndices = value;
        }
    }
    public SizeAwareList<float> Keys { get; }
    public uint NumIndices
    {
        get => (uint)(Indices?.Count ?? 0);
        set
        {
            if (value == NumIndices)
                return;

            if (value < NumIndices)
            {
                while (NumIndices > value)
                    Indices.RemoveAt(Indices.Count - 1);
            }
            else
            {
                while (NumIndices < value)
                    Indices.Add(default);
            }
            NumKeys = value;
        }
    }
    public SizeAwareList<uint> Indices { get; }

    public override byte[] DataBytes
    {
        get
        {
            var data = new List<byte>((int)DataLength);

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumKeys));
            foreach (var key in Keys)
                data.AddRange(BitConverter.GetBytes(key));
            foreach (var index in Indices)
                data.AddRange(BitConverter.GetBytes(index));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) * NumKeys + sizeof(uint) * NumIndices;

    public ExpressionChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadP3DString(), ListHelper.ReadArray(br.ReadInt32, br.ReadSingle, out var numKeys), ListHelper.ReadArray(numKeys, br.ReadUInt32))
    {
    }

    public ExpressionChunk(uint version, string name, IList<float> keys, IList<uint> indices) : base(ChunkID, name)
    {
        _version = version;
        Keys = CreateSizeAwareList(keys, Keys_CollectionChanged);
        Indices = CreateSizeAwareList(indices, Indices_CollectionChanged);
    }
    
    private void Keys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(Keys));
    
    private void Indices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(Indices));

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Keys.Count != Indices.Count)
            yield return new InvalidP3DException(this, $"{nameof(Keys)} and {nameof(Indices)} must have equal counts.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(NumKeys);
        foreach (var key in Keys)
            bw.Write(key);
        foreach (var index in Indices)
            bw.Write(index);
    }

    protected override Chunk CloneSelf() => new ExpressionChunk(Version, Name, Keys, Indices);
}
