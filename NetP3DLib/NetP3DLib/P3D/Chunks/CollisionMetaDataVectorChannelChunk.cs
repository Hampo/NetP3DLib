using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMetaDataVectorChannelChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Meta_Data_Vector_Channel;

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
    public SizeAwareList<Vector3> Values { get; }

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
                data.AddRange(BinaryExtensions.GetBytes(value));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(ushort) * NumIndices + sizeof(float) * 3 * NumValues;

    public CollisionMetaDataVectorChannelChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadP3DString(), ListHelper.ReadArray(br.ReadInt32, br.ReadUInt16, out var numIndices), ListHelper.ReadArray(numIndices, br.ReadVector3))
    {
    }

    public CollisionMetaDataVectorChannelChunk(uint version, string name, IList<ushort> indices, IList<Vector3> values) : base(ChunkID, name)
    {
        Version = version;
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

    protected override Chunk CloneSelf() => new CollisionMetaDataVectorChannelChunk(Version, Name, Indices, Values);
}