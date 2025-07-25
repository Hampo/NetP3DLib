using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMetaDataVectorChannelChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Meta_Data_Vector_Channel;
    
    public uint Version { get; set; }
    public uint NumIndices
    {
        get => (uint)Indices.Count;
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
    public List<ushort> Indices { get; } = [];
    public uint NumValues
    {
        get => (uint)Values.Count;
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
    public List<Vector3> Values { get; } = [];

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

    public CollisionMetaDataVectorChannelChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        var numFrames = br.ReadInt32();
        Indices = new(numFrames);
        Values = new(numFrames);
        for (var i = 0; i < numFrames; i++)
            Indices.Add(br.ReadUInt16());
        for (var i = 0; i < numFrames; i++)
            Values.Add(br.ReadVector3());
    }

    public CollisionMetaDataVectorChannelChunk(uint version, string name, IList<ushort> indices, IList<Vector3> values) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Indices.AddRange(indices);
        Values.AddRange(values);
    }

    public override void Validate()
    {
        if (Indices.Count != Values.Count)
            throw new InvalidDataException($"{nameof(Indices)} and {nameof(Values)} must have equal counts.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(NumIndices);
        foreach (var index in Indices)
            bw.Write(index);
        foreach (var value in Values)
            bw.Write(value);
    }

    internal override Chunk CloneSelf() => new CollisionMetaDataVectorChannelChunk(Version, Name, Indices, Values);
}