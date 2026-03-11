using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationChannelCountChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation_Channel_Count;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint ChannelChunkID { get; set; }
    public uint NumNumKeys
    {
        get => (uint)(NumKeys?.Count ?? 0);
        set
        {
            if (value == NumNumKeys)
                return;

            if (value < NumNumKeys)
            {
                while (NumNumKeys > value)
                    NumKeys.RemoveAt(NumKeys.Count - 1);
            }
            else
            {
                while (NumNumKeys < value)
                    NumKeys.Add(default);
            }
        }
    }
    public SizeAwareList<ushort> NumKeys { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(ChannelChunkID));
            data.AddRange(BitConverter.GetBytes(NumNumKeys));
            foreach (var key in NumKeys)
                data.AddRange(BitConverter.GetBytes(key));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(ushort) * NumNumKeys;

    public AnimationChannelCountChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadUInt32(), ListHelper.ReadArray(br.ReadInt32(), br.ReadUInt16))
    {
    }

    public AnimationChannelCountChunk(uint version, uint channelChunkID, IList<ushort> numKeys) : base(ChunkID)
    {
        Version = version;
        ChannelChunkID = channelChunkID;
        NumKeys = CreateSizeAwareList(numKeys);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(ChannelChunkID);
        bw.Write(NumNumKeys);
        foreach (var key in NumKeys)
            bw.Write(key);
    }

    protected override Chunk CloneSelf() => new AnimationChannelCountChunk(Version, ChannelChunkID, NumKeys);
}
