using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation_Group;
    private const uint BaseSize = 40;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint GroupID { get; set; }
    public uint NumChannels => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(GroupID));
            data.AddRange(BitConverter.GetBytes(NumChannels));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public AnimationGroupChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        GroupID = br.ReadUInt32();
        var numChannels = br.ReadUInt32();
    }

    public AnimationGroupChunk(uint version, string name, uint groupID) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        GroupID = groupID;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(GroupID);
        bw.Write(NumChannels);
    }

    protected override Chunk CloneSelf() => new AnimationGroupChunk(Version, Name, GroupID);

    internal uint CalculateMemorySize(AnimationChunk.Platform platform, uint size)
    {
        size = ((size + 7u) & ~7u) + BaseSize;
        size = ((size + 3u) & ~3u) + NumChannels * 4;

        foreach (var intChannel in GetChunksOfType<IntegerChannelChunk>())
            size = intChannel.CalculateMemorySize(platform, size);

        foreach (var float1Channel in GetChunksOfType<Float1ChannelChunk>())
            size = float1Channel.CalculateMemorySize(platform, size);

        foreach (var float2Channel in GetChunksOfType<Float2ChannelChunk>())
            size = float2Channel.CalculateMemorySize(platform, size);

        foreach (var vector1DOFChannel in GetChunksOfType<Vector1DOFChannelChunk>())
            size = vector1DOFChannel.CalculateMemorySize(platform, size);

        foreach (var vector2DOFChannel in GetChunksOfType<Vector2DOFChannelChunk>())
            size = vector2DOFChannel.CalculateMemorySize(platform, size);

        foreach (var vector3DOFChannel in GetChunksOfType<Vector3DOFChannelChunk>())
            size = vector3DOFChannel.CalculateMemorySize(platform, size);

        foreach (var compressedQuaternionChannel in GetChunksOfType<CompressedQuaternionChannelChunk>())
            size = compressedQuaternionChannel.CalculateMemorySize(platform, size);

        foreach (var quaternionChannel in GetChunksOfType<QuaternionChannelChunk>())
            size = quaternionChannel.CalculateMemorySize(platform, size);

        foreach (var entityChannel in GetChunksOfType<EntityChannelChunk>())
            size = entityChannel.CalculateMemorySize(platform, size);

        foreach (var booleanChannel in GetChunksOfType<BooleanChannelChunk>())
            size = booleanChannel.CalculateMemorySize(platform, size);

        foreach (var colourChannel in GetChunksOfType<ColourChannelChunk>())
            size = colourChannel.CalculateMemorySize(platform, size);

        /*foreach (var eventChannel in GetChunksOfType<EventChannelChunk>())
            size = eventChannel.CalculateMemorySize(platform, size);*/

        return size;
    }
}
