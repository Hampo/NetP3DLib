using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
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
        get => (uint)(KeyFrameIds?.Count ?? 0);
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
    public SizeAwareList<uint> KeyFrameIds { get; }
    public uint NumKeyFrameCounts
    {
        get => (uint)(KeyFrameCounts?.Count ?? 0);
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
    public SizeAwareList<uint> KeyFrameCounts { get; }

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
        var keyFrameIds = new uint[numKeyFrameIds];
        for (int i = 0; i < numKeyFrameIds; i++)
            keyFrameIds[i] = br.ReadUInt32();
        KeyFrameIds = CreateSizeAwareList(keyFrameIds);
        var keyFrameCounts = new uint[numKeyFrameIds];
        for (int i = 0; i < numKeyFrameIds; i++)
            keyFrameCounts[i] = br.ReadUInt32();
        KeyFrameCounts = CreateSizeAwareList(keyFrameCounts);
    }

    public VertexAnimKeyFrameListChunk(uint version, IList<uint> keyFrameIds, IList<uint> keyFrameCounts) : base(ChunkID)
    {
        Version = version;
        KeyFrameIds = CreateSizeAwareList(keyFrameIds);
        KeyFrameCounts = CreateSizeAwareList(keyFrameCounts);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (KeyFrameIds.Count != KeyFrameCounts.Count)
            yield return new InvalidP3DException(this, $"{nameof(KeyFrameIds)} and {nameof(KeyFrameCounts)} must have equal counts.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumKeyFrameIds);
        foreach (var keyFrameId in KeyFrameIds)
            bw.Write(keyFrameId);
        foreach (var keyFrameCount in KeyFrameCounts)
            bw.Write(keyFrameCount);
    }

    protected override Chunk CloneSelf() => new VertexAnimKeyFrameListChunk(Version, KeyFrameIds, KeyFrameCounts);
}