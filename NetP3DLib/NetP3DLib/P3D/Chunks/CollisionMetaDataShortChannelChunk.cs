using NetP3DLib.IO;
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
public class CollisionMetaDataShortChannelChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Meta_Data_Short_Channel;
    
    public uint Version { get; set; }
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
            NumValues = value;
        }
    }
    public SizeAwareList<ushort> Indices { get; }
    public uint NumValues
    {
        get => (uint)(Values?.Count ?? 0);
        set
        {
            if (value == NumValues)
                return;

            if (value < NumValues)
            {
                while (NumValues > value)
                    Values.RemoveAt(Values.Count - 1);
            }
            else
            {
                while (NumValues < value)
                    Values.Add(default);
            }
            NumIndices = value;
        }
    }
    public SizeAwareList<ushort> Values { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumIndices));
            foreach (var frame in Indices)
                data.AddRange(BitConverter.GetBytes(frame));
            foreach (var value in Values)
                data.AddRange(BitConverter.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(ushort) * NumIndices + sizeof(ushort) * NumValues;

    public CollisionMetaDataShortChannelChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        var numFrames = br.ReadInt32();
        var indices = new ushort[numFrames];
        for (var i = 0; i < numFrames; i++)
            indices[i] = br.ReadUInt16();
        Indices = CreateSizeAwareList(indices);
        var values = new ushort[numFrames];
        for (var i = 0; i < numFrames; i++)
            values[i] = br.ReadUInt16();
        Values = CreateSizeAwareList(values);
    }

    public CollisionMetaDataShortChannelChunk(uint version, string name, IList<ushort> indices, IList<ushort> values) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        Indices = CreateSizeAwareList(indices);
        Values = CreateSizeAwareList(values);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (Indices.Count != Values.Count)
            yield return new InvalidP3DException(this, $"{nameof(Indices)} and {nameof(Values)} must have equal counts.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
        foreach (var value in Values)
            bw.Write(value);
    }

    protected override Chunk CloneSelf() => new CollisionMetaDataShortChannelChunk(Version, Name, Indices, Values);
}