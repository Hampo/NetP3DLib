using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class VertexAnimKeyFrameListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Vertex_Anim_Key_Frame_List;
    
    public uint Version { get; set; }
    public uint NumKeyFrameIds
    {
        get => (uint)KeyFrameIds.Count;
        set
        {
            if (value == NumKeyFrameIds)
                return;

            if (value < NumKeyFrameIds)
            {
                while (NumKeyFrameIds > value)
                    KeyFrameIds.RemoveAt(KeyFrameIds.Count - 1);
            }
            else
            {
                while (NumKeyFrameIds < value)
                    KeyFrameIds.Add(default);
            }
            NumKeyFrameCounts = value;
        }
    }
    public List<uint> KeyFrameIds { get; } = [];
    public uint NumKeyFrameCounts
    {
        get => (uint)KeyFrameCounts.Count;
        set
        {
            if (value == NumKeyFrameCounts)
                return;

            if (value < NumKeyFrameCounts)
            {
                while (NumKeyFrameCounts > value)
                    KeyFrameCounts.RemoveAt(KeyFrameCounts.Count - 1);
            }
            else
            {
                while (NumKeyFrameCounts < value)
                    KeyFrameCounts.Add(default);
            }
            NumKeyFrameIds = value;
        }
    }
    public List<uint> KeyFrameCounts { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumKeyFrameIds));
            foreach (var keyFrameId in KeyFrameIds)
                data.AddRange(BitConverter.GetBytes(keyFrameId));
            foreach (var keyFrameCount in KeyFrameCounts)
                data.AddRange(BitConverter.GetBytes(keyFrameCount));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) * NumKeyFrameIds + sizeof(uint) * NumKeyFrameCounts;

    public VertexAnimKeyFrameListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numKeyFrameIds = br.ReadInt32();
        KeyFrameIds.Capacity = numKeyFrameIds;
        KeyFrameCounts.Capacity = numKeyFrameIds;
        for (int i = 0; i < numKeyFrameIds; i++)
            KeyFrameIds.Add(br.ReadUInt32());
        for (int i = 0; i < numKeyFrameIds; i++)
            KeyFrameCounts.Add(br.ReadUInt32());
    }

    public VertexAnimKeyFrameListChunk(uint version, IList<uint> keyFrameIds, IList<uint> keyFrameCounts) : base(ChunkID)
    {
        Version = version;
        KeyFrameIds.AddRange(keyFrameIds);
        KeyFrameCounts.AddRange(keyFrameCounts);
    }

    public override void Validate()
    {
        if (KeyFrameIds.Count != KeyFrameCounts.Count)
            throw new InvalidDataException($"{nameof(KeyFrameIds)} and {nameof(KeyFrameCounts)} must have equal counts.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumKeyFrameIds);
        foreach (var keyFrameId in KeyFrameIds)
            bw.Write(keyFrameId);
        foreach (var keyFrameCount in KeyFrameCounts)
            bw.Write(keyFrameCount);
    }

    internal override Chunk CloneSelf() => new VertexAnimKeyFrameListChunk(Version, KeyFrameIds, KeyFrameCounts);
}